using FluentValidation;
using FluentValidation.Results;
using iVectorOne_Admin_Api.Features.Shared;
using static iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test.Handler;
using System.Net.Http.Headers;
using Intuitive.Helpers.Security;
using Intuitive;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public class Handler : IRequestHandler<Request, ResponseBase>
    {
        private readonly AdminContext _context;
        private IValidator<Request> _validator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecretKeeper _secretKeeper;

        public Handler(AdminContext context, IValidator<Request> validator, ISecretKeeper secretKeeper, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _validator = validator;
            _httpClientFactory = httpClientFactory;
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
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

            if (string.IsNullOrEmpty(account.EncryptedPassword))
            {
                //TODO
                //response.Ok(new ResponseModel { Success = true, Status = "Sorry, there are no tests configured for this supplier." });
                return response;
            }

            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                response.BadRequest("", validationResult.ToDictionary());
                return response;
            }

            var results = await ExecuteSearch(string.Join(", ", request.SearchCriteria.Properties), request.SearchCriteria.ArrivalDate, account.Login, account.EncryptedPassword);

            response.Ok(new ResponseModel { Success = true, Results = results.SearchResult });

            return response;
        }

        private async Task<SearchResponse> ExecuteSearch(string properties, DateTime searchdate, string login, string password)
        {
            var response = new SearchResponse();

            try
            {
                var requestURI = $"https://api.ivectorone.com/v2/properties/search?ArrivalDate={searchdate:yyyy-MM-dd}&duration=7&properties={properties}&rooms=(2,0,0)&dedupeMethod=none";

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestURI);

                var httpClient = _httpClientFactory.CreateClient();

                var x = _secretKeeper.Decrypt(password);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{_secretKeeper.Decrypt(password)}")));
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                var searchResult = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var results = JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertySearch.Response>(searchResult);
                    if (results != null && results.PropertyResults.Count > 0)
                    {
                        var searchResults = new List<SearchResult>();

                        foreach (var property in results.PropertyResults)
                        {
                            foreach (var roomType in property.RoomTypes)
                            {
                                searchResults.Add(new SearchResult
                                {
                                    Supplier = roomType.Supplier,
                                    RoomCode = roomType.RateCode,
                                    RoomType = roomType.SupplierRoomType,
                                    MealBasis = roomType.MealBasisCode,
                                    Currency = roomType.CurrencyCode,
                                    NonRefundable = roomType.NonRefundable
                                });
                            }

                        }
                        response.SearchResult = searchResults;

                        //response.SearchStatus = SearchStatusEnum.Ok;
                    }
                    else
                    {
                        //response.SearchStatus = SearchStatusEnum.NoResults;
                    }
                }
                else
                {
                    //response.Message = $"{httpResponseMessage.StatusCode} {searchResult}";
                    //response.SearchStatus = SearchStatusEnum.NotOk;
                }
            }
            catch (Exception ex)
            {
                //response.Message = ex.Message;
                //response.SearchStatus = SearchStatusEnum.Exception;
            }

            return response;
            ;
        }

        internal record SearchResponse
        {
            internal List<SearchResult> SearchResult { get; set; }
            //internal SearchStatusEnum SearchStatus { get; set; }

            //internal string Message { get; set; } = "";
        }
    }
}
