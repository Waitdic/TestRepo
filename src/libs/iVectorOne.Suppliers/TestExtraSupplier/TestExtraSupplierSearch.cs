namespace iVectorOne.Suppliers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;
    using iVectorOne.Extra;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models.Extra;

    public class TestExtraSupplierSearch : IThirdPartySearch, ISingleSource
    {
        private ITestExtraSupplierSettings _settings;

        private readonly HttpClient _httpClient;
        private readonly ISqlFactory _sqlFactory;

        public string Source => ThirdParties.TESTEXTRASUPPLIER;

        public TestExtraSupplierSearch(ITestExtraSupplierSettings settings,
            HttpClient httpClient,
            ISqlFactory sqlFactory)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _sqlFactory = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory));
        }

        public Task<List<Request>> BuildSearchRequestsAsync(ExtraSearchDetails searchDetails, LocationMapping location)
        {
            (string, string) tpLocations = ((string, string))GetThirdPartyLocations(searchDetails, location);
            bool returnResults = tpLocations.Item1.ToLower() == "airport1" && tpLocations.Item2.ToLower() == "resort1";

            System.Threading.Thread.Sleep(_settings.SearchTimeMilliseconds(searchDetails));
            return Task.FromResult(new List<Request>() { new Request() {
                EndPoint = "",
                ExtraInfo = returnResults,
                Method=RequestMethod.GET} });
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(ExtraSearchDetails searchDetails)
        {
            return false;
        }

        public TransformedExtraResultCollection TransformResponse(List<Request> requests, ExtraSearchDetails searchDetails, LocationMapping location)
        {
            var transformedResults = new TransformedExtraResultCollection();

            foreach (var request in requests)
            {
                if (request.ExtraInfo != null && !(bool)(request.ExtraInfo))
                {
                    return transformedResults;
                }

                for (int i = 0; i < 3; i++)
                {
                    var extra = new TransformedExtraResult()
                    {
                        TPSessionID = $"TPSession_{i}",
                        SupplierReference = $"SupplierRef_{i}",
                        ExtraVehicle = $"Car_{i}", //TODO : Confirm if it should be Transfer or Extra here
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
                        OutboundExtraMinutes = 55, //TODO : Confirm if it should be Transfer or Extra here
                        ReturnExtraMinutes = 55 //TODO : Confirm if it should be Transfer or Extra here
                    };

                    transformedResults.TransformedResults.Add(extra);
                }
            }

            return transformedResults;
        }

        public bool ValidateSettings(ExtraSearchDetails searchDetails)
        {
            return true;
        }
        private object GetThirdPartyLocations(ExtraSearchDetails searchDetails, LocationMapping location)
        {
            return (location.DepartureData, location.ArrivalData);
        }
    }
}
