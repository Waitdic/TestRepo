namespace ThirdParty.CSSuppliers.Netstorming
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using ThirdParty;
    using ThirdParty.CSSuppliers.Netstorming.Models;
    using ThirdParty.CSSuppliers.Netstorming.Models.Common;
    using ThirdParty.CSSuppliers.Netstorming.Resources;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;
    using Cancellation = ThirdParty.Models.Property.Booking.Cancellation;
    using RoomDetails = iVector.Search.Property.RoomDetails;

    public class NetstormingSearch : IThirdPartySearch, IMultiSource
    {
        private readonly INetstormingSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;

        public NetstormingSearch(
            INetstormingSettings settings,
            ITPSupport support,
            ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public List<string> Sources => NetstormingSupport.NetstormingSources;

        public bool SupportsNonRefundableTagging => false;

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            foreach (var room in searchDetails.RoomDetails.Where(r => r.Infants > 0))
            {
                restrictions = true;
            }

            return restrictions;
        }

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var requests = new List<Request>();

            // perform a search for each resort
            foreach (var resortSplit in resortSplits)
            {
                string resortCode = resortSplit.ResortCode;

                // get a list of all the possible combinations of room request and store them in an array so that an availability request can be sent for each combo
                var roomRequests = NetstormingSupport.GetRoomRequests(searchDetails.RoomDetails);
                int roomRequest = 1;

                foreach (var roomCombinationRequest in roomRequests)
                {
                    string nationality = await _support.TPNationalityLookupAsync(source, searchDetails.NationalityCode);
                    var xmlRequest = BuildRequest(searchDetails, roomCombinationRequest, resortCode,
                        searchDetails.ArrivalDate, searchDetails.DepartureDate,
                        _settings.Actor(searchDetails, source), _settings.User(searchDetails, source),
                        _settings.Password(searchDetails, source), _settings.Version(searchDetails, source), nationality);

                    var request = new Request
                    {
                        EndPoint = _settings.GenericURL(searchDetails, source),
                        Method = RequestMethod.POST,
                    };
                    request.SetRequest(xmlRequest);
                    requests.Add(request);
                    roomRequest += 1;
                }
            }

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            string source = resortSplits.First().ThirdPartySupplier;

            var allResponses =
                from request in requests
                where request.Success
                select _serializer.DeSerialize<NetstormingAvailabilityResponse>(request.ResponseXML);

            var roomTypesXml = new XmlDocument();
            roomTypesXml.LoadXml(NetstormingRes.NetstormingRoomTypes);

            var roomTypes = roomTypesXml.SelectNodes("RoomTypeDetails/Contracts/contract")!
                .Cast<XmlNode>()
                .ToDictionary(
                    contract => contract.Attributes!["contract_id"].Value,
                    contract => contract.Attributes!["name"].Value);

            var rooms = GetRoomsConfig(searchDetails.RoomDetails);

            transformedResults.TransformedResults.AddRange(allResponses.SelectMany(r => GetResultFromResponse(r, searchDetails, rooms, roomTypes, source)));

            return transformedResults;
        }

        private IEnumerable<TransformedResult> GetResultFromResponse(
            NetstormingAvailabilityResponse response,
            IThirdPartyAttributeSearch searchDetails,
            List<RoomConfig> roomsConfig,
            IReadOnlyDictionary<string, string> roomTypes,
            string source)
        {
            var transformedResults = new List<TransformedResult>();

            string language = _settings.LanguageCode(searchDetails, source);
            var cancellations = GetCancellationsFromResponse(response.Response);

            foreach (var hotel in response.Response.Hotels.Hotel)
            {
                string tpKey = hotel.Code;
                string searchNumber = response.Response.Search.Number;

                foreach (var agreement in hotel.Agreement)
                {
                    var agreementInfo = cancellations[agreement];
                    string agreementId = agreement.Id;
                    string currencyCode = agreement.Currency;
                    string mealBasis = agreement.RoomBasis;
                    string roomTypeId = agreement.C_type;
                    string roomDescription = agreement.RoomType;
                    string roomBasis = agreement.RoomBasis;
                    string tprMealBasis = agreement.MealBasis;
                    string available = agreement.Available;
                    string special = agreement.Special;
                    bool nonRefundable = agreementInfo.NonRefundable;

                    // we need to go through each of the room types which has been added to the end of the search response xml
                    // and then match the correct room types so that we can work out which rooms go with which room type, this also
                    // helps work out the different room combinations

                    foreach (var roomConfig in roomsConfig)
                    {
                        foreach (var configRoomType in roomConfig.Rooms)
                        {
                            transformedResults.AddRange(from room in agreement.Room
                                let childAge = room.Age.ToSafeInt()
                                where available == "true" && room.Type == configRoomType.RoomCode &&
                                      room.Extrabed == configRoomType.ExtraBeds && childAge == configRoomType.ChildAge &&
                                      room.Cot == configRoomType.ExtraCot
                                let price = room.Price
                                let amount = price.Sum(p =>
                                    p.RoomPrice.Nett.ToSafeDecimal() + p.CotPrice.Nett.ToSafeDecimal() +
                                    p.ExtraBedPrice.Nett.ToSafeDecimal())
                                let roomType = GetRoomTypeDescription(language, room.Type, roomDescription, roomTypes[roomTypeId], source)
                                let roomCancellations = from cancellation in agreementInfo.Cancellations
                                    select new Cancellation
                                    {
                                        StartDate = cancellation.StartDate,
                                        EndDate = cancellation.EndDate,
                                        Amount = (cancellation.Amount * amount / 100).ToSafeDecimal()
                                    }
                                select new TransformedResult
                                {
                                    TPKey = tpKey,
                                    CurrencyCode = currencyCode,
                                    PropertyRoomBookingID = roomConfig.Prbid,
                                    Amount = amount,
                                    TPReference =
                                        $"{room.Type}_{room.Extrabed}_{childAge}_{room.Cot}_{tpKey}_{amount:#.##}_{hotel.City}_{hotel.Name}_{agreementId}_{roomBasis}_{tprMealBasis}_{roomTypeId}_{available}_{special}_{searchNumber}",
                                    RoomType = roomType,
                                    MealBasisCode = mealBasis,
                                    NonRefundableRates = nonRefundable,
                                    Cancellations = roomCancellations.ToList()
                                });
                        }
                    }
                }
            }

            return transformedResults;
        }

        private string GetRoomTypeDescription(
            string language,
            string roomTypeCode,
            string roomDescription,
            string roomTypeDetail,
            string source)
        {
            string prefix = string.Empty;
            bool isDefault = false;

            switch (language)
            {
                case "IT":
                    prefix = roomTypeCode switch
                    {
                        "sgl" => "Singolo",
                        "dbl" => "Doppio",
                        "trp" => "Triplo",
                        "qud" => "Quadruplo",
                        "tsu" => "Doppia uso singola",
                        "twn" => "Gemello",
                        _ => prefix
                    };
                    break;
                case "ES":
                    prefix = roomTypeCode switch
                    {
                        "sgl" => "Solo",
                        "dbl" => "Doble",
                        "trp" => "Triple",
                        "qud" => "Patio",
                        "tsu" => "Dobles de uso individual",
                        "twn" => "Gemelo",
                        _ => prefix
                    };
                    break;
                case "FR":
                    prefix = roomTypeCode switch
                    {
                        "sgl" => "Seule",
                        "dbl" => "Doubles",
                        "trp" => "Triple",
                        "qud" => "Quadruple",
                        "tsu" => "Jumeaux pour usage exclusif",
                        "twn" => "Jumeau",
                        _ => prefix
                    };
                    break;
                default:
                    prefix = roomTypeCode switch
                    {
                        "sgl" => "Single",
                        "dbl" => "Double",
                        "trp" => "Triple",
                        "qud" => "Quadruple",
                        "tsu" => "Twin for sole use",
                        "twn" => "Twin",
                        _ => prefix
                    };

                    isDefault = true; // language is not IT, ES or FR
                    break;
            }

            return isDefault && source is "Netstorming" or "NetstormingTP"
                ? $"{prefix} {roomDescription}"
                : $"{prefix} {roomTypeDetail}";
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        private static List<RoomConfig> GetRoomsConfig(RoomDetails roomDetails)
        {
            var roomsConfig = new List<RoomConfig>();

            int roomNumber = 1;
            foreach (var room in roomDetails)
            {
                var roomConfig = new RoomConfig
                {
                    // put on an associated room number
                    Prbid = roomNumber
                };

                roomsConfig.Add(roomConfig);

                // add the types of room that could be produced for the occupancy
                var roomTypes = CalculateOccupancyWithRoomType(room);

                foreach (string roomType in roomTypes)
                {
                    string[] splitItems = roomType.Split('_');

                    var roomTypeConfig = new RoomType
                    {
                        RoomCode = splitItems[0],
                        ExtraBeds = splitItems[1],
                        ChildAge = splitItems[2].ToSafeInt(),
                        ExtraCot = splitItems[3]
                    };

                    roomConfig.Rooms.Add(roomTypeConfig);
                }

                roomNumber += 1;
            }

            return roomsConfig;
        }

        private static List<string> CalculateOccupancyWithRoomType(RoomDetail room)
        {
            var oRoomCombo = new List<string>();
            int iPassengerCount = room.Adults + room.Children;
            string infantString = room.Infants == 1 ? "true" : "false";

            switch (iPassengerCount)
            {
                case 1:
                {
                    if (room.Infants == 0)
                    {
                        oRoomCombo.Add("sgl_false_0_false");
                        oRoomCombo.Add("tsu_false_0_false");
                    }
                    else if (room.Infants == 1)
                    {
                        oRoomCombo.Add("tsu_false_0_Y");
                    }

                    break;
                }

                case 2:
                {
                    oRoomCombo.Add($"twn_false_0_{infantString}");
                    oRoomCombo.Add($"dbl_false_0_{infantString}");
                    break;
                }

                case 3:
                {
                    oRoomCombo.Add($"trp_false_0_{infantString}");
                    break;
                }

                case 4:
                {
                    oRoomCombo.Add($"qud_false_0_{infantString}");
                    break;
                }
            }

            return oRoomCombo;
        }

        protected virtual XmlDocument BuildRequest(IThirdPartyAttributeSearch searchDetails,
            NetstormingSupport.RoomCombo roomCombinationRequest, string resortCode, DateTime propertyArrivalDate,
            DateTime propertyDepartureDate, string actor, string user, string password, string version, string nationality)
        {
            var request = new NetstormingAvailabilityRequest
            {
                Header = NetstormingSupport.Header(actor, user, password, version),
                Query =
                {
                    Type = "availability",
                    Product = "hotel",
                    Filters = new[] {"AVAILONLY"},
                    Checkin = {Date = propertyArrivalDate.ToString("yyyy-MM-dd")},
                    Checkout = {Date = propertyDepartureDate.ToString("yyyy-MM-dd")},
                    City = {Code = resortCode},
                    Details = roomCombinationRequest.ToArray(),
                    Nationality = nationality
                }
            };

            return NetstormingSupport.Serialize(request, _serializer);
        }

        private static Dictionary<Agreement, AgreementCancellationInfo> GetCancellationsFromResponse(Response response)
        {
            var cancellationsInfo = new Dictionary<Agreement, AgreementCancellationInfo>();

            foreach (var hotel in response.Hotels.Hotel)
            {
                foreach (var agreement in hotel.Agreement)
                {
                    var cancellationInfo = new AgreementCancellationInfo();

                    if (agreement.Policies.Policy.Any())
                    {
                        foreach (var policy in agreement.Policies.Policy)
                        {
                            cancellationInfo.Cancellations.Add(SetupCancellation(policy));

                            if (policy.Percentage == "100" && policy.From.ToSafeDate() <= DateTime.Now.Date)
                            {
                                cancellationInfo.NonRefundable = true;
                            }
                        }
                    }
                    else
                    {
                        var cancellation = new Cancellation
                        {
                            StartDate = agreement.Deadline.Value.ToSafeDate(),
                            EndDate = new DateTime(2099, 12, 30),
                            Amount = 100
                        };

                        if (cancellation.StartDate < DateTime.Now.Date)
                            cancellation.StartDate = DateTime.Now.Date;

                        cancellationInfo.Cancellations.Add(cancellation);
                    }

                    cancellationsInfo.Add(agreement, cancellationInfo);
                }
            }

            return cancellationsInfo;
        }

        private static Cancellation SetupCancellation(Policy policy)
        {
            var oCancellation = new Cancellation
            {
                StartDate = policy.From.ToSafeDate(),
                Amount = policy.Percentage.ToSafeDecimal(),
                EndDate = new DateTime(2099, 12, 30)
            };

            if (oCancellation.StartDate < DateTime.Now.Date)
                oCancellation.StartDate = DateTime.Now.Date;
            return oCancellation;
        }

        private class AgreementCancellationInfo
        {
            public Cancellations Cancellations { get; } = new();

            public bool NonRefundable { get; set; }
        }

        private class RoomConfig
        {
            public List<RoomType> Rooms { get; set; } = new();

            public int Prbid { get; set; }
        }

        private class RoomType
        {
            public string RoomCode { get; set; } = string.Empty;

            public string ExtraBeds { get; set; } = string.Empty;

            public int ChildAge { get; set; }

            public string ExtraCot { get; set; } = string.Empty;
        }
    }
}