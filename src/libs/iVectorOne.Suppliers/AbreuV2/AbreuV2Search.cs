namespace iVectorOne.Suppliers.AbreuV2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.AbreuV2.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class AbreuV2Search : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IAbreuV2Settings _settings;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.ABREUV2;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        #region Constructors

        public AbreuV2Search(IAbreuV2Settings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            //'Set up usual stuff
            var requests = new List<Request>();

            string languageId = _settings.LanguageID(searchDetails);
            string password = _settings.Password(searchDetails);
            string user = _settings.User(searchDetails);
            string databaseName = _settings.DatabaseName(searchDetails);
            string target = _settings.Target(searchDetails);
            string dateStamp = AbreuV2.Now();
            string xmlRequest;

            foreach (var resort in resortSplits)
            {
                var sbRequest = new OTA_HotelAvailRQ
                {
                    HotelSearch =
                    {
                        Currency = { Code = _settings.CurrencyCode(searchDetails) },
                        HotelRef = { HotelCode = string.Join(",", resort.Hotels.Select(x => x.TPKey).ToArray()) },
                        DateRange =
                        {
                            Start = searchDetails.ArrivalDate.ToString(Constant.XmlDateFormat),
                            End = searchDetails.ArrivalDate.AddDays(searchDetails.Duration).ToString(Constant.XmlDateFormat)
                        },
                        RoomCandidates = searchDetails.RoomDetails.Select(roomDetail =>
                        {
                            var guest = new List<Guest>();

                            if (roomDetail.Adults > 0)
                            {
                                guest.Add(new Guest { AgeCode = Constant.AgeCodeAdult, Count = roomDetail.Adults });
                            }

                            guest.AddRange(roomDetail.ChildAges
                                .Select(childAge => new Guest { AgeCode = Constant.AgeCodeChild, Count = 1, Age = $"{childAge}" }));

                            if (roomDetail.Infants > 0)
                            {
                                guest.Add(new Guest { AgeCode = Constant.AgeCodeChild, Count = roomDetail.Infants, Age = $"{Constant.InfantDefaultAge}"});
                            }

                            return new RoomCandidate
                            {
                                RPH = $"{roomDetail.PropertyRoomBookingID}",
                                Guests = guest,
                            };
                        }).ToList()
                    }
                };

                var soapEnvelope = new Envelope<OTA_HotelAvailRQ>
                {
                    Header =
                    {
                        Security = AbreuV2.BuildCredentials(_settings, searchDetails)
                    },
                    Body =
                    {
                        Content = sbRequest
                    }
                };

                xmlRequest = XmlHelper.CleanRequest(_serializer.Serialize(soapEnvelope));

                //'Build Request Object
                var request = new Request
                {
                    EndPoint = _settings.SearchURL(searchDetails),
                    Method = RequestMethod.POST,
                    UseGZip = true
                };
                request.SetRequest(xmlRequest);

                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var hotelAvailRsList = new List<OTA_HotelAvailRS>();
            var transformedResults = new TransformedResultCollection();

            string languageId = _settings.LanguageID(searchDetails);

            int totalRooms = searchDetails.Rooms;

            hotelAvailRsList = requests.Select(request =>
            {
                var responseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);
                var hotelAvailRs = _serializer.DeSerialize<OTA_HotelAvailRS>(responseXml);
                return hotelAvailRs;
            }).ToList();

            foreach (var hotelAvalRs in hotelAvailRsList)
            {
                foreach (var hotel in hotelAvalRs.Hotels)
                {
                    //all rooms should have same meal plan
                    //select Cheapest rate for single request
                    var roomMealsRates = hotel.Rooms
                        .SelectMany(room => room.RPH.Split(',')
                            .SelectMany(rph => room.RoomRates
                                .Select(roomRate => new
                                {
                                    Room = room,
                                    RoomId = rph,
                                    RoomRate = roomRate,
                                    Amount = roomRate.Total.Amount.ToSafeDecimal(),
                                    roomRate.Total.Currency
                                })))
                        .GroupBy(x => new { x.Room.RoomType, x.RoomRate.MealPlan });

                    var chepestMealPlan = roomMealsRates
                        .Select(x => new { MealRate = x, Amount = x.Sum(room => room.Amount), MealPlan = x.Key.MealPlan })
                        .OrderBy(x => x.Amount)
                        .First()
                        .MealPlan;

                    //if single room, get all meal types
                    //if multiple rooms get rates with chepest meal plan
                    var roomSets = (totalRooms == 1)
                        ? roomMealsRates.Select(g => g.First())
                        : roomMealsRates.Where(g => g.Key.MealPlan == chepestMealPlan).SelectMany(x => x);

                    var transfomedRooms = roomSets.Select((x, i) =>
                    {
                        string cancelPenalties = _serializer.Serialize(x.RoomRate.CancelPenalties).OuterXml;

                        return new TransformedResult
                        {
                            TPKey = hotel.Info.HotelCode,
                            CurrencyCode = hotel.BestPrice.Currency,
                            RoomType = x.Room.RoomType.Name,
                            RoomTypeCode = x.Room.RoomType.Code,
                            MealBasisCode = x.RoomRate.MealPlan,
                            Amount = x.Amount,
                            NonRefundableRates = string.Equals(x.RoomRate.CancelPenalties.NonRefundable, "1"),
                            SpecialOffer = "",
                            PropertyRoomBookingID = x.RoomId.ToSafeInt(),
                            TPReference = $"{x.RoomRate.BookingCode}|{cancelPenalties}"
                        };
                    }).ToList();

                    transformedResults.TransformedResults.AddRange(transfomedRooms);
                }
            }

            return transformedResults;
        }
    }
}