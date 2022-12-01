namespace iVectorOne.Suppliers.Polaris
{
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.Polaris.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PolarisSearch : IThirdPartySearch, ISingleSource
    {
        public string Source => ThirdParties.POLARIS;

        public IPolarisSettings _settings;

        public PolarisSearch(IPolarisSettings settings)
        {
            _settings = settings;
        }

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = resortSplits.SelectMany(resort => 
            {
                return BuildResortRequests(searchDetails, resort);
            }).ToList();

            return Task.FromResult(requests);
        }

        public List<Request> BuildResortRequests(SearchDetails searchDetails, ResortSplit resort) 
        {
            string basicAuthToken = Convert.ToBase64String(new UTF8Encoding().GetBytes(
                $"{_settings.User(searchDetails)}:{_settings.Password(searchDetails)}"));

            var requests = Enumerable.Range(0, _settings.SplitMultiRoom(searchDetails) 
                                                        ? searchDetails.Rooms 
                                                        : 1).Select(splitIdx =>
            {
                var availRequest = new AvailRequest
                {
                    HotelAvailability = {
                    SearchAvail =
                    {
                        CheckIn = searchDetails.ArrivalDate.ToString(Constant.DateFormat),
                        CheckOut = searchDetails.ArrivalDate.AddDays(searchDetails.Duration).ToString(Constant.DateFormat),
                        Destination =
                        {
                            HotelCodes = resort.Hotels.Select(x => x.TPKey).ToList(),
                            Location = new Location
                            {
                                DestinationCode = resort.ResortCode,
                                Type = Constant.LocationType.Empty,
                            }
                        },
                        Market = string.IsNullOrEmpty(searchDetails.SellingCountry)
                                        ? string.Empty
                                        : searchDetails.SellingCountry,

                        Rooms = searchDetails.RoomDetails
                        .Where((room, roomIdx) => !_settings.SplitMultiRoom(searchDetails)
                                                  || roomIdx == splitIdx)
                        .Select((room, roomIdx) => new RoomRequest
                        {
                            Index = roomIdx + 1,
                            PassengerAges = Enumerable.Range(0, room.Adults)
                                                      .Select(_ => Constant.DefaultAdultAge)
                                            .Concat(room.ChildAges)
                                            .Concat(Enumerable.Range(0, room.Infants)
                                                              .Select(_ => Constant.DefaultInfantAge))
                                            .ToList()
                        }).ToList()
                    }
                },
                    Token = DateTime.UtcNow.ToString()
                };

                var requestString = JsonConvert.SerializeObject(availRequest);

                var request = new Request()
                {
                    EndPoint = _settings.SearchURL(searchDetails),
                    Method = RequestMethod.POST,
                    Source = Source,
                    ExtraInfo = splitIdx + 1,
                    ContentType = ContentTypes.Application_json,
                    UseGZip = _settings.UseGZip(searchDetails)
                };
                request.SetRequest(requestString);
                request.Headers.AddNew("Authorization", "Basic " + basicAuthToken);
                return request;
            }).ToList();
            return requests;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var now = DateTime.Now;
            var nowDay = new DateTime(now.Year, now.Month, now.Day);
            var responses = requests.SelectMany(requests =>
            {
                var availResp = JsonConvert.DeserializeObject<AvailResponse>(requests.ResponseString);

                return availResp.Hotels.SelectMany(hotel =>
                {
                    return hotel.RoomRates.Where(roomRate => roomRate.Status == Constant.Status.Ok)
                                          .SelectMany(roomRate =>
                    {
                        var canxs = TransformCancellations(roomRate.CancellationPolicies);
                        var isRateNonRefundable = canxs.Where(x => x.StartDate <= nowDay 
                                                                && x.Amount >= roomRate.Pricing.Net.Price).Any();
                        var excludeNRF = _settings.ExcludeNRF(searchDetails);

                        var cc = new Cancellations();
                        cc.AddRange(canxs);

                        return roomRate.Rooms.Where(x => !(excludeNRF && isRateNonRefundable)).Select(roomInfo =>
                        {
                            return new TransformedResult
                            {
                                TPKey = hotel.HotelCode,
                                RateCode = roomRate.RateId,
                                CurrencyCode = roomRate.Pricing.Currency,
                                RoomType = roomInfo.Name,
                                RoomTypeCode = roomInfo.Id,
                                PropertyRoomBookingID = _settings.SplitMultiRoom(searchDetails)
                                                        ? requests.ExtraInfo.ToSafeInt()
                                                        : roomInfo.Index,
                                MinimumPrice = roomInfo.Pricing.Sell.Price,
                                Amount = roomInfo.Pricing.Net.Price,                                
                                MealBasisCode = roomRate.Meal.Id,
                                TPReference = roomRate.BookToken,
                                Cancellations = cc,
                                NonRefundableRates = isRateNonRefundable                            
                            };
                        });
                    });
                });
            }).ToList();

            var resultCollection = new TransformedResultCollection();
            resultCollection.TransformedResults.AddRange(responses);
            return resultCollection;
        }

        public static List<Cancellation> TransformCancellations(List<CancellationPolicy> cancellations) 
        {
            var canxs = cancellations.Select(cancellation => new Cancellation
            {
                StartDate = cancellation.From.ToSafeDate(),
                EndDate = cancellation.To.ToSafeDate(),
                Amount = cancellation.Pricing.Net.Price
            }).ToList();
            return canxs;
        }
    }
}
