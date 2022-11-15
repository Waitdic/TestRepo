namespace iVectorOne.Suppliers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using iVectorOne.Transfer;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;


    public class NullTestTransferSupplierSearch : IThirdPartySearch, ISingleSource
    {
        private INullTestTransferSupplierSettings _settings;

        public string Source => ThirdParties.NULLTESTTRANSFERSUPPLIER;

        public NullTestTransferSupplierSearch(INullTestTransferSupplierSettings settings)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            var tpLocation = GetThirdPartyLocations<int>(searchDetails, location);

            System.Threading.Thread.Sleep(_settings.SearchTimeMilliseconds(searchDetails));
            return Task.FromResult(new List<Request>() { new Request() {
                EndPoint = "",
                ExtraInfo = searchDetails,
                Method=RequestMethod.GET} });
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, LocationMapping location)
        {
            var transformedResults = new TransformedTransferResultCollection();

            for (int i = 0; i < 3; i++)
            {
                var transfer = new TransformedTransferResult()
                {
                    TPSessionID = $"TPSession_{i}",
                    SupplierReference = $"SupplierRef_{i}",
                    TransferVehicle = $"Car_{i}",
                    ReturnTime = "15:00",
                    VehicleCost = 150,
                    AdultCost = 100,
                    ChildCost = 50,
                    CurrencyCode = "CAD",
                    VehicleQuantity = 1,
                    Cost = 200,
                    BuyingChannelCost = 0,
                    OutboundInformation = $"OutboundInformation_{i}",
                    ReturnInformation = $"ReturnInformation_{i}",
                    OutboundCost = 100,
                    ReturnCost = 100,
                    OutboundXML = "",
                    ReturnXML = "",
                    OutboundTransferMinutes = 55,
                    ReturnTransferMinutes = 55
                };

                transformedResults.TransformedResults.Add(transfer);
            }

            return transformedResults;
        }

        public Task<T> GetThirdPartyLocations<T>(TransferSearchDetails searchDetails, LocationMapping location)
        {
            return default;
        }

        public bool SearchRestrictions(TransferSearchDetails searchDetails)
        {
            return false;
        }

    }
}
