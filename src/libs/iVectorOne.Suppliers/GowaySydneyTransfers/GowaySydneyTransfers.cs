namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using Intuitive;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;

    public class GowaySydneyTransfers : TourPlanTransfersBase
    {
        public override string Source => ThirdParties.GOWAYSYDNEYTRANSFERS;

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;

        public GowaySydneyTransfers(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger) : base(settings, httpClient, logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }
    }
}
