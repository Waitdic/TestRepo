namespace iVectorOne.Suppliers.GowaySydneyExtras
{
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.TourPlanExtras;
    using Microsoft.Extensions.Logging;
    using System.Net.Http;

    public class GowaySydneyExtrasSearch : TourPlanExtrasSearchBase
    {
        public override string Source => ThirdParties.GOWAYSYDNEYEXTRAS;

        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanExtrasSearchBase> _logger;

        public GowaySydneyExtrasSearch(HttpClient httpClient, ISerializer serializer, ILogger<TourPlanExtrasSearchBase> logger) : base(httpClient, serializer, logger)
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }
    }
}
