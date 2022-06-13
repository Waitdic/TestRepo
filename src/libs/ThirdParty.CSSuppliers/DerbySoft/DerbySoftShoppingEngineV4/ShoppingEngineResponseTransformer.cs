namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Newtonsoft.Json;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4.Models;
    using ThirdParty.CSSuppliers.DerbySoft.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Support;

    public class ShoppingEngineResponseTransformer : ISearchResponseTransformer
    {
        private readonly TransformedResultBuilder _resultBuilder;

        public ShoppingEngineResponseTransformer(IDerbySoftSettings settings, string source)
        {
            _resultBuilder = new TransformedResultBuilder(settings, source);
        }

        public IEnumerable<TransformedResult> TransformResponses(List<Request> requests)
        {
           return requests
                .Where(r => r.Success)
                .SelectMany(r => TransformResponse(
                    (SearchExtraHelper)r.ExtraInfo,
                    JsonConvert.DeserializeObject<DerbySoftShoppingEngineV4SearchResponse>(r.ResponseString, DerbySoftSupport.GetJsonSerializerSettings())));
        }

        public IEnumerable<TransformedResult> TransformResponse(
            SearchExtraHelper searchExtraHelper,
            DerbySoftShoppingEngineV4SearchResponse searchResponse)
        {
            var searchDetails = searchExtraHelper.SearchDetails;
            if (searchResponse?.AvailableHotels is null || !searchResponse.AvailableHotels.Any())
            {
                yield break;
            }

            foreach (var hotel in searchResponse.AvailableHotels.Where(h => IsValidHotel(h) && IsActiveHotel(h)))
            {
                foreach (var roomRate in hotel.AvailableRoomRates.Where(r => IsValidRoom(r) && HasInventory(r)))
                {
                    var transformedResult =
                        _resultBuilder.BuildTransformedResult(
                            searchDetails,
                            searchExtraHelper.ExtraInfo.ToSafeInt(),
                            hotel.HotelId,
                            searchResponse.Header.Token,
                            roomRate);

                    if (transformedResult is object)
                    {
                        yield return transformedResult;
                    }
                }
            }
        }

        private static bool IsValidHotel(AvailableHotel hotel) =>
            hotel.HotelId is object
            && hotel.AvailableRoomRates is object
            && hotel.StayRange is object
            && hotel.StayRange.CheckIn.HasValue
            && hotel.StayRange.CheckIn != DateTime.MinValue
            && hotel.StayRange.CheckOut.HasValue
            && hotel.StayRange.CheckOut != DateTime.MinValue;

        private static bool IsActiveHotel(AvailableHotel hotel) =>
            hotel.Status.HasValue && hotel.Status == HotelStatus.Active;

        private static bool IsValidRoom(RoomRate roomRate) =>
            !string.IsNullOrWhiteSpace(roomRate.MealPlan)
            && roomRate.RoomCriteria is object
            && roomRate.RoomCriteria.RoomCount == 1
            && roomRate.RoomCriteria.AdultCount > 0
            && roomRate.RoomCriteria.ChildCount >= 0
            && (roomRate.DailyCostBeforeTax is object && roomRate.DailyCostBeforeTax.Any())
            || (roomRate.DailyCostAfterTax is object && roomRate.DailyCostAfterTax.Any())
            && !string.IsNullOrWhiteSpace(roomRate.Currency)
            && roomRate.Inventory is object;

        private static bool HasInventory(RoomRate roomRate) =>
            roomRate.Inventory != 0;
    }
}
