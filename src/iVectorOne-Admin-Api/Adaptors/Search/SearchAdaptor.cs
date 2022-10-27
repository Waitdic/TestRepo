using Intuitive.Helpers.Security;
using iVectorOne_Admin_Api.Data.Models;
using iVectorOne_Admin_Api.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace iVectorOne_Admin_Api.Adaptors.Search
{
    public class SearchAdaptor : IAdaptor<Request, Response>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SearchAdaptor> _logger;
        private readonly ISecretKeeper _secretKeeper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SearchAdaptor(IHttpClientFactory httpClientFactory, ILogger<SearchAdaptor> logger, ISecretKeeper secretKeeper, IServiceScopeFactory serviceScopeFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _secretKeeper = secretKeeper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Response> Execute(Request request)
        {
            var response = new Response();

            try
            {
                var requestURI = $"https://api.ivectorone.com/v2/properties/search" +
                    $"?ArrivalDate={request.Searchdate:yyyy-MM-dd}" +
                    $"&duration=7&properties={request.Properties}" +
                    $"&rooms={request.RoomRequest}" +
                    $"&dedupeMethod={request.DedupeMethod}";

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestURI);

                var httpClient = _httpClientFactory.CreateClient();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{request.Login}:{_secretKeeper.Decrypt(request.Password)}")));

                 var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                var searchResult = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertySearch.Response>(searchResult);

                    if (result != null && result.PropertyResults.Count > 0)
                    {
                        response.SearchStatus = Response.SearchStatusEnum.Ok;
                        response.SearchResult = result;
                    }
                    else
                    {
                        response.SearchStatus = Response.SearchStatusEnum.NoResults;
                    }
                }
                else
                {
                    response.Information = $"{httpResponseMessage.StatusCode} {searchResult}";
                    response.SearchStatus = Response.SearchStatusEnum.NotOk;
                }
            }
            catch (Exception ex)
            {
                var activity = Activity.Current;
                response.Information = $"{ex.Message} : {activity?.GetTraceId()}";
                response.SearchStatus = Response.SearchStatusEnum.Exception;

                _logger.LogError(ex, "Unexpected error executing search request.");
            }

            try
            {
                var propertySearchResults = new FireForgetSearchResponse
                {
                    SearchResponse = response.SearchResult,
                    Information = response.Information,
                    SearchStatus = response.SearchStatus.ToString(),
                    FireForgetSearchResponseKey = request.RequestKey.ToString()
                };

                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AdminContext>();
                context.FireForgetSearchResponses.Add(propertySearchResults);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving search response.");
            }

            return response;
        }
    }
}
