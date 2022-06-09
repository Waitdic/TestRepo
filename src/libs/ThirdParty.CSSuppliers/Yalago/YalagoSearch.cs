namespace ThirdParty.CSSuppliers
{
#pragma warning disable CS8618 
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using Newtonsoft.Json;
    using ThirdParty.Search.Models;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Support;
    using ThirdParty.Results;
    using ThirdParty.CSSuppliers.Models.Yalago;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Logging;

    public class YalagoSearch : IThirdPartySearch
    {
        #region "Constructors"

        private readonly IYalagoSettings _settings;

        private readonly ITPSupport _support;

        public YalagoSearch(IYalagoSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        #endregion

        #region "Properties"

        public string Source => ThirdParties.YALAGO;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails)
        {
            return false;
        }

        #endregion

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var searchHelper = new SearchHelper();
            var apiKey = _settings.API_Key(searchDetails);
            string uniqueCode = Source;
            searchHelper.ResortSplit = resortSplits.FirstOrDefault();
            if (resortSplits.Count() > 1)
            {
                uniqueCode = string.Format("{0}_{1}", Source, resortSplits.FirstOrDefault().ResortCode);
            }
            searchHelper.UniqueRequestID = uniqueCode;
            foreach (var searchResortSplit in resortSplits)
            {
                var hotelIDList = new List<int>();

                foreach (var oHotel in searchResortSplit.Hotels)
                {
                    hotelIDList.Add(oHotel.TPKey.ToSafeInt());
                }
                searchHelper.SearchDetails = searchDetails;
                searchHelper.ResortSplit = searchResortSplit;

                YalagoAvailabilityRequest availabilityRequest = GetAvailabilityRequest(searchDetails, hotelIDList, searchHelper.ResortSplit.ResortCode);

                var request = new Request()
                {
                    Accept = "application/gzip",
                    EndPoint = _settings.SearchURL(searchDetails),
                    Method = eRequestMethod.POST,
                    TimeoutInSeconds = 100,
                    UseGZip = _settings.UseGZip(searchDetails),
                    ContentType = "application/json",
                    ExtraInfo = searchHelper,
                    KeepAlive = true,
                    Source = Source
                };
                request.SetRequest(JsonConvert.SerializeObject(availabilityRequest));

                request.Headers.Add("X-Api-Key", apiKey);

                requests.Add(request);

            }
            return requests;
        }

        private YalagoAvailabilityRequest GetAvailabilityRequest(SearchDetails searchDetails, List<int> hotelIds, string resortCode)
        {
            List<YalagoAvailabilityRequest.Room> roomList = new List<YalagoAvailabilityRequest.Room>();

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                YalagoAvailabilityRequest.Room room = new YalagoAvailabilityRequest.Room()
                {
                    Adults = roomDetail.Adults,
                    ChildAges = roomDetail.ChildAndInfantAges().ToArray()
                };

                roomList.Add(room);

            }

            string sourceMarket = _support.TPCountryCodeLookup(ThirdParties.YALAGO, searchDetails.SellingCountry);

            if (string.IsNullOrEmpty(sourceMarket))
            {
                sourceMarket = _settings.CountryCode(searchDetails);
            }
            var getPackagePrice = searchDetails.OpaqueSearch && _settings.ReturnOpaqueRates(searchDetails);
            YalagoAvailabilityRequest yalagoAvailabilityRequest = new YalagoAvailabilityRequest()
            {
                CheckInDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                CheckOutDate = searchDetails.DepartureDate.ToString("yyyy-MM-dd"),
                EstablishmentIds = hotelIds.ToArray(),
                Rooms = roomList.ToArray(),
                SourceMarket = sourceMarket,
                Culture = _settings.Language(searchDetails),
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

                        var cancellations = new List<ThirdParty.Models.Property.Booking.Cancellation>();
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

                            cancellations.Add(new ThirdParty.Models.Property.Booking.Cancellation()
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

                        if (_settings.ExcludeNonRefundable(searchDetails))
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

        public class SearchHelper : SearchExtraHelper
        {
            public ResortSplit ResortSplit;
            public List<string> PropertyIDs = new List<string>();
        }
    }
}