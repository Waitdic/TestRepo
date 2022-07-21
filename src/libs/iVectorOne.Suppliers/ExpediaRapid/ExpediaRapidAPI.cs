namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;
    using ThirdParty.Models.Property.Booking;

    public class ExpediaRapidAPI : IExpediaRapidAPI
    {
        private readonly HttpClient _httpclient;
        private readonly ILogger<ExpediaRapidAPI> _logger;

        public ExpediaRapidAPI(HttpClient httpClient, ILogger<ExpediaRapidAPI> logger)
        {
            _httpclient = httpClient;
            _logger = logger;
        }

        public async Task<TResponse> GetDeserializedResponseAsync<TResponse>(PropertyDetails propertyDetails, Request request) where TResponse : IExpediaRapidResponse<TResponse>, new()
        {
            string responseString = await GetResponseAsync(propertyDetails, request);
            var response = new TResponse();

            (bool valid, response) = response.GetValidResults(responseString, (int)request.ResponseStatusCode);

            if (!valid)
            {
                response = default;
            }

            return response;
        }

        public async Task<string> GetResponseAsync(PropertyDetails propertyDetails, Request request)
        {
            await request.Send(_httpclient, _logger);

            propertyDetails.AddLog(request.LogFileName, request);

            return request.ResponseString;
        }
    }
}