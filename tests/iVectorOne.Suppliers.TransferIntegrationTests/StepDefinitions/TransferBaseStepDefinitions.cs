
using iVectorOne.SDK.V2;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    public class TransferBaseStepDefinitions
    {
        private WebApplicationFactory<Program> _factory;
        protected HttpClient _httpClient;
        protected ScenarioContext _scenarioContext;

        protected const string AgentID = "GOWAUD";
        protected const string Password = "koala12";
        protected const string URL = "https://pa-gowsyd.nx.tourplan.net/iCom_Test/servlet/conn";

        private const string ReqUsername = "GoWayTest";
        private const string ReqPassword = "GoWayTest";

        public TransferBaseStepDefinitions(ScenarioContext scenarioContext)
        {
            _factory = new WebApplicationFactory<Program>();
            _scenarioContext = scenarioContext;
            _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,

            });
            _httpClient.Timeout = TimeSpan.FromMinutes(5);

        }

        public StringContent CreateRequest(TransferRequestBase requestObj)
        {
            if (requestObj != null)
            {

                var request = JsonConvert.SerializeObject(requestObj);

                var authenticationString = $"{ReqUsername}:{ReqPassword}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                return new StringContent(request, Encoding.UTF8, "application/json");

            }

            return null;
        }



    }
}
