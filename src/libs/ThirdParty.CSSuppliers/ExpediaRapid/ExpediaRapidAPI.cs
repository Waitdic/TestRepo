namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using System.Net.Http;
    using Newtonsoft.Json;
    using ThirdParty.Constants;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;
    using Microsoft.Extensions.Logging;

    public class ExpediaRapidAPI : IExpediaRapidAPI
    {
        private readonly HttpClient _httpclient;
        private readonly ILogger<ExpediaRapidAPI> _logger;

        public ExpediaRapidAPI(HttpClient httpClient, ILogger<ExpediaRapidAPI> logger)
        {
            _httpclient = httpClient;
            _logger = logger;
        }

        public TResponse GetDeserializedResponse<TResponse>(PropertyDetails propertyDetails, Intuitive.Net.WebRequests.Request request) where TResponse : IExpediaRapidResponse<TResponse>, new()
        {
            string responseString = GetResponse(propertyDetails, request);
            var response = new TResponse();

            (bool valid, response) = response.GetValidResults(responseString, (int)request.ResponseStatusCode);

            if (!valid)
            {
                response = default;
            }

            return response;
        }

        public string GetResponse(PropertyDetails propertyDetails, Intuitive.Net.WebRequests.Request request)
        {
            request.Send(_httpclient, _logger);

            propertyDetails.Logs.AddNew(ThirdParties.EXPEDIARAPID, $"{request.LogFileName} Request", request.RequestString);
            propertyDetails.Logs.AddNew(ThirdParties.EXPEDIARAPID, $"{request.LogFileName} Response", request.ResponseString);

            return request.ResponseString;
        }

    }

}