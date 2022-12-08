namespace iVectorOne.Suppliers
{
#pragma warning disable CS8618 
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Newtonsoft.Json;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Models.Yalago;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class YalagoSearch : IThirdPartySearch, ISingleSource
    {
        #region Constructors

        private readonly IYalagoSettings _settings;

        private readonly ITPSupport _support;

        public YalagoSearch(IYalagoSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.YALAGO;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var apiKey = _settings.APIKey(searchDetails);

            foreach (var searchResortSplit in resortSplits)
            {
                var hotelIDList = new List<int>();

                foreach (var oHotel in searchResortSplit.Hotels)
                {
                    hotelIDList.Add(oHotel.TPKey.ToSafeInt());
                }

                var availabilityRequest = await GetAvailabilityRequestAsync(searchDetails, hotelIDList, searchResortSplit.ResortCode);

                var request = new Request()
                {
                    Accept = "application/gzip",
                    EndPoint = _settings.SearchURL(searchDetails),
                    Method = RequestMethod.POST,
                    TimeoutInSeconds = 100,
                    UseGZip = _settings.UseGZip(searchDetails),
                    ContentType = "application/json",
                    KeepAlive = true,
                    Source = Source
                };
                request.SetRequest(JsonConvert.SerializeObject(availabilityRequest));

                request.Headers.Add("X-Api-Key", apiKey);

                requests.Add(request);

            }
            return requests;
        }

        private async Task<YalagoAvailabilityRequest> GetAvailabilityRequestAsync(SearchDetails searchDetails, List<int> hotelIds, string resortCode)
        {
            var roomList = new List<YalagoAvailabilityRequest.Room>();

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                var room = new YalagoAvailabilityRequest.Room()
                {
                    Adults = roomDetail.Adults,
                    ChildAges = roomDetail.ChildAndInfantAges().ToArray()
                };

                roomList.Add(room);
            }

            string sourceMarket = await _support.TPCountryCodeLookupAsync(ThirdParties.YALAGO, searchDetails.SellingCountry, searchDetails.Account.AccountID);

            if (string.IsNullOrEmpty(sourceMarket))
            {
                sourceMarket = _settings.SourceMarket(searchDetails);
            }
            var getPackagePrice = searchDetails.OpaqueSearch && _settings.ReturnOpaqueRates(searchDetails);
            YalagoAvailabilityRequest yalagoAvailabilityRequest = new YalagoAvailabilityRequest()
            {
                CheckInDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                CheckOutDate = searchDetails.DepartureDate.ToString("yyyy-MM-dd"),
                EstablishmentIds = hotelIds.ToArray(),
                Rooms = roomList.ToArray(),
                SourceMarket = sourceMarket,
                Culture = _settings.LanguageCode(searchDetails),
                GetPackagePrice = getPackagePrice,
                GetTaxBreakDown = true,
                GetLocalCharges = true
            };

            if (!hotelIds.Any())
            {
                yalagoAvailabilityRequest.LocationId = resortCode.ToSafeInt();
            }
            return yalagoAvailabilityRequest;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allResponses = new List<YalagoAvailabilityResponse>();

            foreach (var request in requests)
            {
                var response = new YalagoAvailabilityResponse();
                bool success = request.Success;

                if (success)
                {
                    response = JsonConvert.DeserializeObject<YalagoAvailabilityResponse>(request.ResponseString);
                    allResponses.Add(response);
                }
            }

            transformedResults.TransformedResults.AddRange(allResponses
                .Where(o => o.Establishments.Any()).SelectMany(r => GetResultFromResponse(searchDetails, r)));

            return transformedResults;
        }

        private List<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, YalagoAvailabilityResponse response)
        {
            List<TransformedResult> transformedResults = new List<TransformedResult>();
            foreach (YalagoAvailabilityResponse.Establishment hotel in response.Establishments)
            {
                var roomDictionary = new Dictionary<string, List<int>>();
                decimal amount;
                string currency;


                foreach (YalagoAvailabilityResponse.Room room in hotel.Rooms)
                {

                    foreach (YalagoAvailabilityResponse.Board board in room.Boards)
                    {
                        if (board.IsBindingPrice) continue;

                        amount = board.netCost.Amount.ToSafeDecimal();
                        currency = board.netCost.Currency;

                        string TPRef = hotel.EstablishmentId.ToSafeString() + "|" + board.Code + "|" +
                                       hotel.establishmentInfo.LocationId.ToSafeString() + "|" + room.Code + "|" +
                                       searchDetails.OpaqueSearch.ToSafeString() + "|" + board.Type.ToSafeString();

                        var cancellations = new List<iVectorOne.Models.Property.Booking.Cancellation>();
                        var orderedCancellations = new List<YalagoAvailabilityResponse.CancellationCharge>();
                        if (board.cancellationPolicy != null)
                        {
                            orderedCancellations = board.cancellationPolicy.CancellationCharges
                            .OrderBy(c => c.ExpiryDate.Date)
                            .ToList();
                        }                        

                        for (int iCanx = 0; iCanx < orderedCancellations.Count; iCanx++)
                        {
                            var cancellation = orderedCancellations[iCanx];
                            var endDate = searchDetails.ArrivalDate;
                            if (iCanx < orderedCancellations.Count - 1)
                            {
                                endDate = orderedCancellations[iCanx + 1].ExpiryDate.Date.AddDays(-1);
                            }

                            cancellations.Add(new iVectorOne.Models.Property.Booking.Cancellation()
                            {
                                Amount = Math.Round(100 * cancellation.charge.Amount / amount, 2),
                                StartDate = cancellation.ExpiryDate.Date,
                                EndDate = endDate
                            });
                        }

                        TransformedResult transformedResult = new TransformedResult()
                        {
                            RoomTypeCode = room.Code,
                            RoomType = room.Description,
                            Amount = amount.ToSafeDecimal(),
                            MinimumPrice = board.IsBindingPrice ? board.grossCost.Amount.ToSafeDecimal() : 0,
                            CurrencyCode = currency,
                            TPReference = TPRef,
                            TPKey = hotel.EstablishmentId.ToSafeString(),
                            PropertyRoomBookingID = board.RequestedRoomIndex + 1,
                            NonRefundableRates = board.NonRefundable,
                            MealBasisCode = board.Type.ToString(),
                            Cancellations = cancellations
                        };

                        if (_settings.ExcludeNRF(searchDetails))
                        {
                            if (transformedResult.NonRefundableRates == false)
                            {
                                transformedResults.Add(transformedResult);
                            }
                        }
                        else
                        {
                            transformedResults.Add(transformedResult);
                        }
                    }
                }
            }
            return transformedResults;
        }
    }
}