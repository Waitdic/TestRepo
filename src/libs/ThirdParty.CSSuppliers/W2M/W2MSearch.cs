namespace ThirdParty.CSSuppliers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers.Helpers.W2M;
    using ThirdParty.CSSuppliers.Xml.W2M;
    using Microsoft.Extensions.Logging;

    public class W2MSearch : IThirdPartySearch
    {
        #region "Properties"

        private readonly IW2MSettings _settings;
        private readonly ISerializer _serializer;

        private readonly SearchRequestBuilder _searchRequestBuilder;

        public string Source => ThirdParties.W2M;

        public bool ResponseHasExceptions(Request request)
        {
            return request.ResponseString.Contains(Constants.ErrorsNode);
        }

        public bool SearchRestrictions(SearchDetails searchDetails)
        {
            return _settings.SplitMultiroom(searchDetails) && searchDetails.Rooms > 1;
        }

        #endregion

        #region "Constructors"

        public W2MSearch(IW2MSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<W2MSearch> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _searchRequestBuilder = new SearchRequestBuilder(_settings, serializer, httpClient, logger);
        }

        #endregion

        #region "Build Search Request"

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var searchRequests = _searchRequestBuilder.BuildSearchRequests(searchDetails, Source, resortSplits);

            return searchRequests;
        }

        #endregion

        #region "Transform Response"

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var resultsCollection = new TransformedResultCollection();

            foreach (var request in requests)
            {
                var responseString = _serializer.CleanSoapDocument(request.ResponseXML.InnerXml);
                if (responseString.Contains(Constants.ErrorsNode))
                {
                    continue;
                }

                var hotelAvailResponse = _serializer.DeSerialize<HotelAvailResponse>(responseString);
                var searchExtraHelper = request.ExtraInfo as SearchExtraHelper;

                if (searchExtraHelper == null)
                {
                    continue;
                }

                var transformedResults = TransformResults(hotelAvailResponse
                    .AvailabilityRS.Results.HotelResultList, searchExtraHelper);

                resultsCollection.TransformedResults.AddRange(transformedResults);
            }
            
            return resultsCollection;
        }

        private static IEnumerable<TransformedResult> TransformResults(
            IEnumerable<HotelAvailResponseHotelResult> hotelResultList, SearchExtraHelper helper)
        {
            foreach (var hotelResult in hotelResultList)
            {
                foreach (var hotelOption in hotelResult.HotelOptions)
                {
                    var totalFixAmounts = hotelOption.Prices.Price.TotalFixAmounts;
                    foreach (var hotelRoom in hotelOption.HotelRooms.HotelRoomList)
                    {
                        var roomTypeCode = hotelRoom.RoomCategory.Type;
                        if (!string.IsNullOrWhiteSpace(hotelRoom.RoomCategory.Text))
                        {
                            roomTypeCode += $"-{hotelRoom.RoomCategory.Text}";
                        }

                        var transformedResult = new TransformedResult
                        {
                            TPKey = hotelResult.Code,
                            PropertyRoomBookingID = helper.ExtraInfo.ToSafeInt(),
                            Cancellations = GetCancellations(hotelOption, totalFixAmounts.Gross, helper).ToList(),
                            CurrencyCode = hotelOption.Prices.Price.Currency,
                            RoomType = hotelRoom.Name,
                            RoomTypeCode = roomTypeCode,
                            Amount = totalFixAmounts.Gross,
                            MealBasisCode = hotelOption.Board.Type,
                            TPReference = hotelOption.RatePlanCode,
                            SpecialOffer = hotelOption.AdditionalElements?.HotelOffers?.HotelOffer?.Description ?? "",
                            AvailableRooms = hotelRoom.AvailRooms,
                            NonRefundableRates = hotelOption.NonRefundable,
                        };

                        yield return transformedResult;
                    }
                }
            }
        }

        private static IEnumerable<Cancellation> GetCancellations(HotelOption hotelOption,
            decimal price, SearchExtraHelper helper) =>
            hotelOption.CancellationPolicy?.PolicyRules?.RuleList
                .Select(policyRule => new Cancellation()
                {
                    StartDate = policyRule.DateFrom != null
                        ? DateTime.ParseExact(policyRule.DateFrom, Constants.DateTimeFormat, CultureInfo.InvariantCulture)
                        : DateTime.Now.Date,
                    EndDate = policyRule.DateTo != null
                        ? DateTime.ParseExact(policyRule.DateTo, Constants.DateTimeFormat, CultureInfo.InvariantCulture)
                        : helper.SearchDetails.ArrivalDate,
                    Amount = W2MHelper.GetCancellationCost(price, helper.SearchDetails.Duration, policyRule)
                }) ?? new List<Cancellation>();

        #endregion
    }
}