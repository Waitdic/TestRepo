namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    using Intuitive;
    using Intuitive.Helpers.Security;
    using Microsoft.AspNetCore.WebUtilities;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ConfigContext _context;
        private readonly ISecretKeeper _secretKeeper;

        public Handler(ConfigContext context, ISecretKeeperFactory secretKeeperFactory)
        {
            _context = Ensure.IsNotNull(context, nameof(context));
            _secretKeeper = Ensure.IsNotNull(secretKeeperFactory, nameof(secretKeeperFactory))
                .CreateSecretKeeper("FireyNebulaIsGod", EncryptionType.Aes, CipherMode.ECB);
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();
            string label = "Sorry, there are no tests configured for this supplier";

            try
            {
                var account = _context.Accounts.Where(a => a.AccountId == request.AccountID)
                    .Include(a => a.AccountSuppliers)
                    .ThenInclude(a => a.Supplier)
                    .FirstOrDefault();

                var dateTimeList = new List<DateTime>
                {
                    DateTime.Today.AddMonths(1),
                    DateTime.Today.AddMonths(3),
                    DateTime.Today.AddMonths(6),
                };

                var requestList = new List<string>();

                var supplier = account.AccountSuppliers.FirstOrDefault(a => a.SupplierId == request.SupplierID).Supplier;

                foreach (var dateTime in dateTimeList)
                {
                    string searchURL = BuildSearchURL(supplier, dateTime);

                    requestList.Add(searchURL);
                }

                foreach (var searchRequest in requestList)
                {
                    HttpClient httpClient = new();

                    var authenticationString = $"{account.Login}:{_secretKeeper.Decrypt(account.EncryptedPassword)}";

                    var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                    var testResponse = await httpClient.GetAsync(searchRequest, cancellationToken);

                    var result = testResponse.Content.ReadAsStringAsync(cancellationToken).Result;
                    var resultClass = JsonSerializer.Deserialize<SupplierTestSearchResponse>(result);

                    if (resultClass != null && resultClass.PropertyResults.Length > 0)
                    {
                        label = "Success. The supplier is returning results";
                        break;
                    }
                    else
                    {
                        label = "Sorry, no results could be found";
                    }
                };

                response.Default(new SupplierTestResponse { Label = label});

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

        private static string BuildSearchURL(Supplier supplier, DateTime arrivalDate)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"ArrivalDate", arrivalDate.ToString("yyyy-MM-dd") },
                {"Duration", "2" },
                {"Rooms", "(2,0,0)" },
                {"Properties", supplier.TestPropertyIDs },
                {"Suppliers", supplier.SupplierName }
            };

            return QueryHelpers.AddQueryString("https://api.ivectorone.com/v2/properties/search?", queryParams);
        }
    }
}
