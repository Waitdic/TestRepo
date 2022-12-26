using Intuitive.Helpers.Extensions;
using iVectorOne.SDK.V2;
using iVectorOne.SDK.V2.TransferSearch;
using Newtonsoft.Json;

namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    [Binding]
    public class TransferSearchApiStepDefinitions : TransferBaseStepDefinitions
    {
        public TransferSearchApiStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        { }

        [Given(@"Create request object for search")]
        public void GivenCreateRequestObjectForSearch()
        {
            int departuteId = 184;
            int arrivalId = 179;

            var requestObj = new Request
            {
                DepartureLocationID = departuteId,
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
        public async Task WhenMakeAPostRequestTo(string p0)
        {

            TransferRequestBase requestObj = (TransferRequestBase)_scenarioContext["RequestObj"];
            var requestContent = CreateRequest(requestObj);

            var response = await _httpClient.PostAsync(p0, requestContent);

            _scenarioContext["ResponseCode"] = response.StatusCode;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                List<TransferResult> transferResults = JsonConvert.DeserializeObject<List<TransferResult>>(result);
                if (transferResults != null && transferResults.Count > 0)
                {
                    //Add first record in scenario context.
                }
                else
                {
                    GivenCreateRequestObjectForSearch();
                }
            }

        }


        [Then(@"the status code should be (.*)")]
        public void ThenTheStatusCodeShouldBe(int p0)
        {
            Assert.Equal(_scenarioContext["ResponseCode"].ToSafeInt(), p0);

            //Now Call prebook scenario.

        }
    }
}
