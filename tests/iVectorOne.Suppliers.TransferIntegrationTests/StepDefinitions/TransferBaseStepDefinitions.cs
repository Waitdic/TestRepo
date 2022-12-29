
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

        private const string ReqUsername = "GoWayTest";
        private const string ReqPassword = "GoWayTest";

        private IConfiguration _config;
        public List<int> locationIds;

        public List<IEnumerable<int>> IVOLocationIds;
        public static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        public TransferBaseStepDefinitions(ScenarioContext scenarioContext)
        {

            _scenarioContext = scenarioContext;


            _config = InitConfiguration();

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

        public List<int> GetLocation(string source)
        {
            if (IVOLocationIds == null)
            {
                locationIds = new List<int>();
                using (SqlConnection connection = new SqlConnection(Convert.ToString(_config["ConnectionStrings:Telemetry"])))
                using (SqlCommand cmd = new SqlCommand(@"select IVOLocationID, Payload  from IVOLocation where Source = @source and Payload Like 'SYD%'", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@source", source));
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int iVOLocationID = reader.GetInt32(reader.GetOrdinal("IVOLocationID"));
                                locationIds.Add(iVOLocationID);
                            }
                        }
                    }
                }

                if (locationIds.Count > 0)
                {
                    int element = locationIds[0];
                    locationIds.RemoveAt(0);
                    IVOLocationIds = GetAllLocationCombinations(locationIds, locationIds.Count, element).ToList();
                }
            }

            if (IVOLocationIds.Count > 0)
            {
                List<int> locations = IVOLocationIds[0].ToList();

                IVOLocationIds.RemoveAt(0);

                return locations;
            }

            return null;
        }

        public IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }

        public IEnumerable<IEnumerable<int>> GetAllLocationCombinations(List<int> list, int length, int element)
        {
            if (length == 1)
            {
                return list.Select(t => new int[] { t });
            }

            return GetAllLocationCombinations(list, length - 1, element)
                    .Select(t => t.Concat(new int[] { element }).Distinct()).ToList();
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
    }
}

