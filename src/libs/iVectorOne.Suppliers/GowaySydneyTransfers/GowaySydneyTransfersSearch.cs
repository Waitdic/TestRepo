namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Services.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;

    public class GowaySydneyTransfersSearch : TourPlanTransfersSearchBase
    {
        public override string Source => ThirdParties.GOWAYSYDNEYTRANSFERS;

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ILocationManagerService _locationManagerService;

        public GowaySydneyTransfersSearch(
            ITourPlanTransfersSettings settings, HttpClient httpClient, ISerializer serializer, ILogger<TourPlanTransfersSearchBase> logger, ILocationManagerService locationManagerService) : base(settings, httpClient, serializer, logger, locationManagerService)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _locationManagerService = Ensure.IsNotNull(locationManagerService, nameof(locationManagerService));
        }
    }
}
