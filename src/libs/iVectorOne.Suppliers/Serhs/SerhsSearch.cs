namespace iVectorOne.Suppliers.Serhs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using Models;
    using Models.Common;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class SerhsSearch : IThirdPartySearch, ISingleSource
    {
        private const string EmptyHotelId = "0";

        private readonly ISerhsSettings _settings;
        private readonly ISerializer _serializer;

        public SerhsSearch(ISerhsSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.SERHS;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string version = _settings.Version(searchDetails);
            string clientCode = _settings.ClientCode(searchDetails);
            string password = _settings.Password(searchDetails);
            string branch = _settings.Branch(searchDetails);
            string tradingGroup = _settings.TradingGroup(searchDetails);
            string languageCode = _settings.LanguageCode(searchDetails);

            var availabilityRequest = new SerhsAvailabilityRequest(version, clientCode, password, branch, tradingGroup, languageCode)
            {
                Period =
                {
                    Start = $"{searchDetails.ArrivalDate:yyyyMMdd}",
                    End = $"{searchDetails.DepartureDate:yyyyMMdd}"
                }
            };

            int i = 0;

            foreach (var resortSplit in resortSplits)
            {
                if (i >= 20) break;
                i++;

                string hotelId = EmptyHotelId;

                if (resortSplit.Hotels.Count == 1 && resortSplits.Count == 1)
                {
                    hotelId = resortSplit.Hotels[0].TPKey;
                }

                if (hotelId != EmptyHotelId)
                {
                    availabilityRequest.SearchCriteria.Add(new Criterion
                    {
                        Type = "1",
                        Code = "accommodationCode",
                        Value = hotelId
                    });
                }
                else
                {
                    availabilityRequest.SearchCriteria.Add(new Criterion
                    {
                        Type = "0",
                        Code = "city",
                        Value = resortSplit.ResortCode
                    });
                }
            }

            availabilityRequest.SearchCriteria.Add(new Criterion
            {
                Type = "2",
                Code = "priceType",
                Value = "3"
            });

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                var room = new Room
                {
                    Adults = roomDetail.Adults, Children = roomDetail.Children + roomDetail.Infants
                };

                foreach (int childAndInfantAge in roomDetail.ChildAndInfantAges())
                {
                    room.Child.Add(new GuestInfo
                    {
                        Age = childAndInfantAge
                    });
                }

                availabilityRequest.Rooms.Add(room);
            }

            var xmlRequest = _serializer.Serialize(availabilityRequest);
            var xmlDeclaration = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
            xmlRequest.InsertBefore(xmlDeclaration, xmlRequest.DocumentElement);

            var request = new Request
            {
                EndPoint = _settings.GenericURL(searchDetails),
                Method = RequestMethod.POST,
                ExtraInfo = searchDetails
            };

            request.SetRequest(xmlRequest);

            return Task.FromResult(new List<Request>{ request });
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var allResponses =
                from request in requests
                where request.Success
                select _serializer.DeSerialize<SerhsAvailabilityResponse>(request.ResponseXML);

            transformedResults.TransformedResults.AddRange(allResponses.SelectMany(r => GetResultFromResponse(r, searchDetails)));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(SerhsAvailabilityResponse response, SearchDetails searchDetails)
        {
            var transformedResults = new List<TransformedResult>();
            bool isMultiRoom = searchDetails.Rooms > 1;

            foreach (var accommodation in response.Accommodations)
            {
                foreach (var concept in accommodation.Concepts)
                {
                    foreach (var board in concept.Boards)
                    {
                        for (int roomNumber = searchDetails.Rooms; roomNumber > 0; --roomNumber)
                        {
                            var offer = board.Offers.FirstOrDefault();
                            bool isNonRefundable = board.Offers.Any(n => n.Code == "99999999999999999999999999999999");
                            decimal amount = ConvertSerhsPrice(board.Price.Amount) / searchDetails.Rooms;

                            transformedResults.Add(
                                new TransformedResult
                                {
                                    TPKey = accommodation.Code,
                                    CurrencyCode = board.Price.CurrencyCode,
                                    PropertyRoomBookingID = roomNumber,
                                    RoomType = concept.Name,
                                    RoomTypeCode = concept.StandardCode,
                                    MealBasisCode = board.Code,
                                    Amount = amount,
                                    TPReference =
                                        $"{accommodation.Type}|{board.Code}|{concept.Code}|{offer?.Code}|{board.Ticket}|{board.CancelPolicyId}|",
                                    NonRefundableRates = isNonRefundable
                                });
                        }

                        if (isMultiRoom)
                        {
                            break;
                        }
                    }

                    if (isMultiRoom)
                    {
                        break;
                    }
                }
            }

            return transformedResults;
        }

        public decimal ConvertSerhsPrice(string price)
        {
            return price.Replace(".", "").Replace(",", ".").ToSafeDecimal();
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Duration > 30;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
    }
}
