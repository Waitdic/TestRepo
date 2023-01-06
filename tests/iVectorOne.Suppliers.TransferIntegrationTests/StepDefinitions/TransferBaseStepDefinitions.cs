
namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    using iVectorOne.SDK.V2;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Text;

    public class TransferBaseStepDefinitions
    {
        protected HttpClient _httpClient;
        protected ScenarioContext _scenarioContext;

        protected const string AgentID = "GOWAUD";
        protected const string Password = "koala12";
        protected const string URL = "https://pa-gowsyd.nx.tourplan.net/iCom_Test/servlet/conn";
        protected const string SearchApi = @"v2/transfers/search";
        protected const string BookApi = @"v2/transfers/book";
        protected const string PrebookApi = @"v2/transfers/prebook";
        protected const string CancelApi = @"v2/transfers/cancel";

        private const string ReqUsername = "GoWayTest";
        private const string ReqPassword = "GoWayTest";
        
        public static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        public TransferBaseStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public void CreateClient(string api = "")
        {
            if (api == "search")
            {
                _httpClient = new WebApplicationFactory<Search.Program>().CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,

                });
                _httpClient.Timeout = TimeSpan.FromMinutes(5); ;
            }
            else
            {
                _httpClient = new WebApplicationFactory<Program>().CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,

                });
                _httpClient.Timeout = TimeSpan.FromMinutes(5); ;
            }
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

        public string GetValue(string key)
        {
            keyValuePairs.TryGetValue(key, out var value);

            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }
            return value;
        }

        public object GetValueFromScenarioConext(string key)
        {
            _scenarioContext.TryGetValue(key, out var value);
            
            return value;
        }
    }
}

