using Intuitive;
using Intuitive.Helpers.Security;
using iVectorOne_Admin_Api.Features.Shared;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Search = iVectorOne.SDK.V2.PropertySearch;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        internal enum SearchStatusEnum { Ok, NoResults, Exception, NotOk };
        private readonly AdminContext _context;
        private readonly ISecretKeeper _secretKeeper;
        private readonly IHttpClientFactory _httpClientFactory;

        public Handler(AdminContext context, ISecretKeeper secretKeeper, IHttpClientFactory httpClientFactory)
        {
            _context = Ensure.IsNotNull(context, nameof(context));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseBase> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = new ResponseBase();

            var account = _context.Accounts.Where(a => a.AccountId == request.AccountID)
                    .Include(a => a.AccountSuppliers)
                    .ThenInclude(a => a.Supplier)
                    .AsNoTracking()
                    .FirstOrDefault();

            if (account == null)
            {
                response.NotFound("Account not found.");
                return response;
            }

            var supplier = account!.AccountSuppliers.FirstOrDefault(a => a.SupplierId == request.SupplierID)!.Supplier;

            if (supplier == null)
            {
                response.NotFound("Supplier not found.");
                return response;
            }

            if (string.IsNullOrEmpty(supplier.TestPropertyIDs) || string.IsNullOrEmpty(account.EncryptedPassword))
            {
                response.Ok(new ResponseModel { Success = true, Status = "Sorry, there are no tests configured for this supplier." });
                return response;
            }


            var status = "Sorry, no results could be found.";
            var searchDate = DateTime.Today.AddDays(1);

            for (int i = 0; i < 3; i++)
            {
                var cancelSearch = false;
                var result = await ExecuteSearch(supplier.TestPropertyIDs, searchDate.AddMonths(i * 3), account.Login, account.EncryptedPassword);
                switch (result.SearchStatus)
                {
                    case SearchStatusEnum.Ok:
                        status = "Success. The supplier is returning results.";
                        break;
                    case SearchStatusEnum.Exception:
                        status = $"Failed with unexpected error: {result.Message}";
                        cancelSearch = true;
                        break;
                    case SearchStatusEnum.NotOk:
                        status = $"Failed with error: {result.Message}";
                        cancelSearch = true;
                        break;
                };

                if (cancelSearch)
                {
                    break;
                }
            }

            response.Ok(new ResponseModel { Success = true, Status = status });
            return response;
        }

        private async Task<SearchResult> ExecuteSearch(string properties, DateTime searchdate, string login, string password)
        {
            var response = new SearchResult();

            try
            {
                var requestURI = $"https://api.ivectorone.com/v2/properties/search?ArrivalDate={searchdate.ToString("yyyy-MM-dd")}&duration=7&properties={properties}&rooms=(2,0,0)";

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestURI);

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromMinutes(3);

                var x = _secretKeeper.Decrypt(password);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{_secretKeeper.Decrypt(password)}")));
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                var searchResult = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertySearch.Response>(searchResult);
                    if (result != null && result.PropertyResults.Count > 0)
                    {
                        response.SearchStatus = SearchStatusEnum.Ok;
                    }
                    else
                    {
                        response.SearchStatus = SearchStatusEnum.NoResults;
                    }
                }
                else
                {
                    response.Message = $"{httpResponseMessage.StatusCode} {searchResult}";
                    response.SearchStatus = SearchStatusEnum.NotOk;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.SearchStatus = SearchStatusEnum.Exception;
            }

            return response;
        }

        internal record SearchResult
        {
            internal SearchStatusEnum SearchStatus { get; set; }

            internal string Message { get; set; } = "";
        }
    }
}