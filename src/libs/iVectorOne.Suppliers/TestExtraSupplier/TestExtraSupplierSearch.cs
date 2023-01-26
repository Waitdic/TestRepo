namespace iVectorOne.Suppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using iVectorOne.Models.Extra;
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

        public Task<List<Request>> BuildSearchRequestsAsync(ExtraSearchDetails searchDetails, List<Extra> extras)
        {
            Extra tpExtra = GetThirdPartyExtras(extras);
            bool returnResults = (tpExtra is not null && !string.IsNullOrEmpty(tpExtra.ExtraName));

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

        public TransformedExtraResultCollection TransformResponse(List<Request> requests, ExtraSearchDetails searchDetails, List<Extra> extras)
        {
            var transformedResults = new TransformedExtraResultCollection();

            foreach (var request in requests)
            {
                if (request.ExtraInfo != null && !(bool)(request.ExtraInfo))
                {
                    return transformedResults;
                }

                foreach (var extra in extras)
                {
                    var result = new TransformedExtraResult()
                    {
                        TPSessionID = $"TPSession_{extra.ExtraName}",
                        SupplierReference = $"SupplierRef_{extra.ExtraName}",
                        ExtraName = extra.ExtraName,
                        ExtraCategory = "testExtraCategory",
                        UseDate = "2023-03-02",
                        UseTime ="10:44",
                        EndDate= "2023-03-10",
                        EndTime= "10:44",
                        CurrencyCode = "GBP",
                        Cost = 200,
                        AdditionalDetails = "testAdditionalDetails"
                    };

                    transformedResults.TransformedResults.Add(result);
                }
            }

            return transformedResults;
        }

        public bool ValidateSettings(ExtraSearchDetails searchDetails)
        {
            return true;
        }
        private Extra GetThirdPartyExtras(List<Extra> extras)
        {
            return extras.FirstOrDefault();
        }
    }
}
