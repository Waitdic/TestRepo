
namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    using iVectorOne.Constants;
    using iVectorOne.SDK.V2;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Text;
    using TechTalk.SpecFlow;

    public class TransferBaseStepDefinitions
    {
        protected HttpClient _httpClient;
        protected ScenarioContext _scenarioContext;

        protected const string GoWayAgentID = "GOWAUD";
        protected const string GoWayPassword = "koala12";
        protected const string GoWayURL = "https://pa-gowsyd.nx.tourplan.net/iCom_Test/servlet/conn";
        protected const string ExoToursAgentID = "goway_test";
        protected const string ExoToursPassword = "fV5bR9uA";
        protected const string ExoToursURL = "https://stage-xml.exotravel.com/Thailand/servlet/conn";
        protected const string PacificAgentID = "GOWNZD";
        protected const string PacificPassword = "kiwi12";
        protected const string PacificURL = "https://pa-pacakl.nx.tourplan.net/iCom_Test/servlet/conn";

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
                var thirdPartySettings = GetThirdPartySettings();
                requestObj.ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", thirdPartySettings.URL },
                    { "AgentID", thirdPartySettings.AgentID},
                    { "Password", thirdPartySettings.Password }
                };
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

        public InjectedTourPlanTransfersSettings GetThirdPartySettings()
        {
            string source = (string)GetValueFromScenarioConext("Source");
            var thirdPartySettings = new InjectedTourPlanTransfersSettings();
            if (!string.IsNullOrEmpty(source))
            {
                switch (source)
                {
                    case ThirdParties.GOWAYSYDNEYTRANSFERS:
                        thirdPartySettings.AgentID = GoWayAgentID;
                        thirdPartySettings.Password = GoWayPassword;
                        thirdPartySettings.URL = GoWayURL;
                        break;
                    case ThirdParties.EXOTOURSTRANSFERS:
                        thirdPartySettings.AgentID = ExoToursAgentID;
                        thirdPartySettings.Password = ExoToursPassword;
                        thirdPartySettings.URL = ExoToursURL;
                        break;
                    case ThirdParties.PACIFICDESTINATIONSTRANSFER:
                        thirdPartySettings.AgentID = PacificAgentID;
                        thirdPartySettings.Password = PacificPassword;
                        thirdPartySettings.URL = PacificURL;
                        break;
                    default:
                        thirdPartySettings.AgentID = GoWayAgentID;
                        thirdPartySettings.Password = GoWayPassword;
                        thirdPartySettings.URL = GoWayURL;
                        break;
                }
            }
            return thirdPartySettings;
        }

        public void SetDataFromStep(string source, Table scenarioData)
        {
            _scenarioContext["DepartureID"] = scenarioData.Rows[0]["DepartureID"];
            _scenarioContext["ArrivalID"] = scenarioData.Rows[0]["ArrivalID"];
            _scenarioContext["BookingReference"] = scenarioData.Rows[0].Keys.Contains("BookingReference") ? scenarioData.Rows[0]["BookingReference"] : string.Empty;
            _scenarioContext["Source"] = source;
        }
    }
}

