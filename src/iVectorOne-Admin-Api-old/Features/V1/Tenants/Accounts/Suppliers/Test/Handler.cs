namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Security;
    using Search = iVectorOne.SDK.V2.PropertySearch;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;
        private readonly ISecretKeeper _secretKeeper;
        private readonly HttpClient _client;

        public Handler(ConfigContext context, ISecretKeeper secretKeeper, HttpClient client)
        {
            _context = Ensure.IsNotNull(context, nameof(context));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _client = Ensure.IsNotNull(client, nameof(client));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();
            string label = "Sorry, there are no tests configured for this supplier";

            try
            {
                var requests = BuildRequest(request);

                foreach (var searchRequest in requests)
                {
                    var result = await _client.SendAsync(searchRequest);
                    ProcessResult(response, result);
                };

                return response;
            }
            catch (Exception)
            {
                response.Default(new SupplierTestResponse
                {
                    Label = label,
                });

                return response;
            }
        }

        private List<HttpRequestMessage> BuildRequest(Request request)
        {
            var account = _context.Accounts.Where(a => a.AccountId == request.AccountID)
                                .Include(a => a.AccountSuppliers)
                                .ThenInclude(a => a.Supplier)
                                .FirstOrDefault();

            var supplier = account!.AccountSuppliers.FirstOrDefault(a => a.SupplierId == request.SupplierID)!.Supplier;

            var dateTimeList = new List<DateTime>
                {
                    DateTime.Today.AddMonths(1),
                    DateTime.Today.AddMonths(3),
                    DateTime.Today.AddMonths(6),
                };

            var requestList = new List<HttpRequestMessage>();

            var room = new Search.RoomRequest
            {
                Adults = 2,
                Children = 0,
                Infants = 0,
            };

            foreach (var dateTime in dateTimeList)
            {
                var propertySearchRequest = new Search.Request
                {
                    ArrivalDate = dateTime,
                    Duration = 2,
                    Properties = supplier.TestPropertyIDs.Split(',').Select(int.Parse).ToList(),
                    Suppliers = new List<string>()
                    {
                        supplier.SupplierName
                    },
                    RoomRequests = new ()
                    {
                        room
                    }
                };

                var propertySearchRequestString = JsonSerializer.Serialize(propertySearchRequest);

                HttpRequestMessage requestMessage = new(HttpMethod.Post, "/v2/properties/search")
                {
                    Content = new StringContent(propertySearchRequestString, Encoding.UTF8, "application/json")
                };
                requestList.Add(requestMessage);
            }

            var authenticationString = $"{account.Login}:{_secretKeeper.Decrypt(account.EncryptedPassword)}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

            return requestList;
        }

        private static async void ProcessResult(Response response, HttpResponseMessage result)
        {
            var searchResult = await result.Content.ReadAsStringAsync();
            var resultClass = JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertySearch.Response>(searchResult);

            if (resultClass != null && resultClass.PropertyResults.Count > 0)
            {
                response.Default(new SupplierTestResponse { Label = "Success. The supplier is returning results" });
            }
            else
            {
                response.Default(new SupplierTestResponse { Label = "Sorry, no results could be found" });
            }
        }
    }
}