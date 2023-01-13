namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;

    public class PacificDestinationsTransfer : TourPlanTransfersBase
    {
        public override string Source => ThirdParties.PACIFICDESTINATIONSTRANSFER;

        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ISerializer _serializer;
        private readonly ITourPlanTransfersSettings _settings;

        public PacificDestinationsTransfer(
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger,
            ISerializer serializer) : base(httpClient, logger, serializer)
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));

        }
    }
}
