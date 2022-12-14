namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;

    public class GowaySydneyTransfers : TourPlanTransfersBase
    {
        public override string Source => ThirdParties.GOWAYSYDNEYTRANSFERS;

        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ISerializer _serializer;
        private readonly ITourPlanTransfersSettings _settings;

        public GowaySydneyTransfers(
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger,
            ISerializer serializer,
            ITourPlanTransfersSettings settings) : base(httpClient, logger, serializer, settings)
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _settings = Ensure.IsNotNull(settings, nameof(settings));

        }
    }
}
