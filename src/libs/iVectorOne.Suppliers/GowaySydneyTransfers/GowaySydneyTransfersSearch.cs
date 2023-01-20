﻿namespace iVectorOne.Suppliers.GowaySydneyTransfers
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

        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ILocationManagerService _locationManagerService;
        private readonly ITourPlanTransfersSettings _settings;

        public GowaySydneyTransfersSearch(HttpClient httpClient, ISerializer serializer, ILogger<TourPlanTransfersSearchBase> logger, ILocationManagerService locationManagerService) : base(serializer, logger, locationManagerService)
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _locationManagerService = Ensure.IsNotNull(locationManagerService, nameof(locationManagerService));
        }
    }
}
