namespace ThirdParty.CSSuppliers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class NullTestSupplier : IThirdPartySearch, ISingleSource
    {
        private readonly INullTestSupplierSettings _settings;

        public string Source => ThirdParties.NULLTESTSUPPLIER;

        public NullTestSupplier(INullTestSupplierSettings settings)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            System.Threading.Thread.Sleep(_settings.SearchTimeMilliseconds(searchDetails));
            return Task.FromResult(new List<Request>() { new Request() {
                EndPoint = "",
                ExtraInfo = searchDetails,
                Method=eRequestMethod.GET} });
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            transformedResults.TransformedResults.AddRange(
                new List<TransformedResult>()
                {
                    new TransformedResult(){
                        MasterID=20213,
                        TPKey="5004855",
                        CurrencyCode = "GBP",
                        PropertyRoomBookingID = 1,
                        RoomTypeCode = "213602289|244042648|37321",
                        RoomType = "Superior Room, 1 King Bed, View (Interior View), 1 King Bed",
                        MealBasisCode = "RO",
                        Adults = 2,
                        Amount = (decimal)858.94,
                        TPReference = "39277253-0ce4-4f55-a991-d2326a33a529|/2.3/properties/20213/rooms/213602289/rates/244042648/price-check?token=F~OjogZzw3JDEtZWBIMVNeSG02B2B3e1gzVFYLCxgEWhVWACQ5BwlWURlUAEkEVHk1fidlNkAMW1lVWFJWUEsOVFEAGgdXUgQYDgYHARlQWAVdAAAABlRVBlw_FARRXFUPCAYGHgQDCgYVAF1RVxVTVQlTGF0GB1xXUFYFU1dVVgY_AAADCAQGFjMNV1IFGzEDs2A4AfA5ZAtWBlMDrmdpAxV-WwsBFSBRUcMwSW9IFkYYQFRAWBQpCl9TIABVFx8bQF4YUkYMG3MIWl4RGERETFheFlxMSBFZBUlt1jM2VwevYzVVUlQHT1sAgzA1jTBhyDU57DUypGE2_jdkxjVnkmdABw1RBQJRAlUcVldcURhVV1QNSFYPAQYfAVFRBQ4FA1ZWVgYLgmArUwgLBkgDA0wBAWNVUgkIXwxXDB8PVAIEXAkGVldqimEzVxYHfRT2XQ==",
                        Discount = (decimal)0.00,
                        SpecialOffer = "Free WiFi",
                        AvailableRooms = 39,
                        CommissionPercentage = 0,
                        DynamicProperty = false,
                        NonPackageAmount = 0,
                        NonRefundableRates = false,
                        FixPrice = false,
                        RegionalTax = "112.69",
                        FreeCanx = false,
                        OnRequest = false,
                        PayLocalAvailable = false,
                        PayLocalRequired = false,
                        MinimumPrice = 0,
                        Adjustments = new List<TransformedResultAdjustment>()
                        {
                            new TransformedResultAdjustment() {
                                AdjustmentID = 0,
                                AdjustmentType = "T",
                                AdjustmentName = "Tax And Service Fee",
                                AdjustmentAmount = (decimal)112.69,
                                PayLocal = true,
                                AdjustmentDescription = ""
                            }
                        }
                    }
                });

            return transformedResults;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }
    }
}