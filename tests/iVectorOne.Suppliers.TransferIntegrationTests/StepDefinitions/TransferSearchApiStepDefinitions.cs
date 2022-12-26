namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.TransferSearch;
    using Newtonsoft.Json;

    [Binding]
    public class TransferSearchApiStepDefinitions : TransferBaseStepDefinitions
    {
        public TransferSearchApiStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Given(@"Create request object for search")]
        public async void GivenCreateRequestObjectForSearch()
        {
            int departureId = 0, arrivalId = 0;
            var locations = GetLocation("gowaysydneytransfers");
            if (locations != null && locations.Count == 2)
            {
                arrivalId = locations[1];
                departureId = locations[0];
            }
            var requestObj = new Request
            {
                DepartureLocationID = departureId,
                ArrivalLocationID = arrivalId,
                DepartureDate = DateTime.Now.AddDays(1),
                OneWay = true,
                Adults = 2,
                Supplier = "gowaysydneytransfers",
                DepartureTime = "10:00",
                ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
            };

            _scenarioContext["RequestObj"] = requestObj;
        }


        [When(@"make a post request to ""([^""]*)""")]
        public async Task WhenMakeAPostRequestTo(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)_scenarioContext["RequestObj"];
            var requestContent = CreateRequest(requestObj);

            var response = await _httpClient.PostAsync(url, requestContent);

            _scenarioContext["ResponseCode"] = (int)response.StatusCode;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<Response>(result);
                List<TransferResult> transferResults = obj.TransferResults;
                if (transferResults != null && transferResults.Count > 0)
                {
                    _scenarioContext["SearchResult"] = transferResults[0];
                }
                else
                {
                    GivenCreateRequestObjectForSearch();
                    await WhenMakeAPostRequestTo("v2/transfers/search");
                }
            }

        }


        [Then(@"the status code should be (.*)")]
        public void ThenTheStatusCodeShouldBe(int status)
        {
            Assert.Equal(_scenarioContext["ResponseCode"], status);
        }

        [Given(@"Create request object for prebook")]
        public void GivenCreateRequestObjectForPrebook()
        {
            var requestObj = new SDK.V2.TransferPrebook.Request
            {
                SupplierReference = "gowaysydneytransfers",
                BookingToken = "Test",
                ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
            };

            _scenarioContext["PrebookObj"] = requestObj;
        }
    }
}
