using Intuitive.Helpers.Security;
using iVectorOne_Admin_Api.Infrastructure.Extensions;
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

        public SearchAdaptor(IHttpClientFactory httpClientFactory, ILogger<SearchAdaptor> logger, ISecretKeeper secretKeeper)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _secretKeeper = secretKeeper;
        }

        public async Task<Response> Execute(Request request, CancellationToken cancellationToken)
        {
            var response = new Response();
            _logger.LogInformation("*** Search Adaptor Start");

            await Task.Delay(120000, cancellationToken);

            //try
            //{
            //    var requestURI = $"https://api.ivectorone.com/v2/properties/search" +
            //        $"?ArrivalDate={request.Searchdate:yyyy-MM-dd}" +
            //        $"&duration=7&properties={request.Properties}" +
            //        $"&rooms={request.RoomRequest}" +
            //        $"&dedupeMethod={request.DedupeMethod}";

            //    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestURI);

            //    var httpClient = _httpClientFactory.CreateClient();

            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{request.Login}:{_secretKeeper.Decrypt(request.Password)}")));

            //    _logger.LogInformation("*** Search Adaptor Before Send");

            //    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

            //    _logger.LogInformation("*** Search Adaptor After Send");

            //    _logger.LogInformation("*** Search Adaptor Before Response");

            //    var searchResult = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            //    _logger.LogInformation("*** Search Adaptor After Response");

            //    if (httpResponseMessage.IsSuccessStatusCode)
            //    {
            //        _logger.LogInformation("*** Search Adaptor Status Success");

            //        _logger.LogInformation("*** Search Adaptor Before Deserialize");

            //        var result = JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertySearch.Response>(searchResult);

            //        _logger.LogInformation("*** Search Adaptor After Deserialize");

            //        if (result != null && result.PropertyResults.Count > 0)
            //        {
            //            response.SearchStatus = Response.SearchStatusEnum.Ok;
            //            response.SearchResult = result;
            //        }
            //        else
            //        {
            //            _logger.LogInformation("*** Search Adaptor Status Not Success");

            //            response.SearchStatus = Response.SearchStatusEnum.NoResults;
            //        }
            //    }
            //    else
            //    {
            //        response.Information = $"{httpResponseMessage.StatusCode} {searchResult}";
            //        response.SearchStatus = Response.SearchStatusEnum.NotOk;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    var activity = Activity.Current;
            //    response.Information = $"{ex.Message} : {activity?.GetTraceId()}";
            //    response.SearchStatus = Response.SearchStatusEnum.Exception;

            //    _logger.LogError(ex, "Unexpected error executing search request.");
            //}

            _logger.LogInformation("*** Search Adaptor End");

            return response;
        }

    }
}
