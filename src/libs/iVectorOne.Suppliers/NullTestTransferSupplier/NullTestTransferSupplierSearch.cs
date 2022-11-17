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
            (string, string) tpLocations = ((string, string))GetThirdPartyLocations(searchDetails, location);
             bool returnResults = tpLocations.Item1.ToLower() == "airport1" && tpLocations.Item2.ToLower() == "resort1";

            System.Threading.Thread.Sleep(_settings.SearchTimeMilliseconds(searchDetails));
            return Task.FromResult(new List<Request>() { new Request() {
                EndPoint = "",
                ExtraInfo = returnResults,
                Method=RequestMethod.GET} });
        }

        public TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, LocationMapping location)
        {
            var transformedResults = new TransformedTransferResultCollection();

            foreach (var request in requests)
            {
                if (request.ExtraInfo != null && !(bool)(request.ExtraInfo))
                {
                    return transformedResults;
                }

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
                        CurrencyCode = "GBP",
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
            }

            return transformedResults;
        }

        public bool SearchRestrictions(TransferSearchDetails searchDetails)
        {
            return false;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public object GetThirdPartyLocations(TransferSearchDetails searchDetails, LocationMapping location)
        {
            return (location.DepartureData, location.ArrivalData);
        }
    }
}
