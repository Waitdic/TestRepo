namespace iVectorOne.Suppliers.ExoToursTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Services.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Constants = TourPlanTransfers.Models.Constant;

    public class ExoToursTransferSearch : TourPlanTransfersSearchBase
    {
        public override string Source => ThirdParties.EXOTOURSTRANSFERS;
        public override string TransferOptText => Constants.TransferOptExoText;

        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ILocationManagerService _locationManagerService;
        private readonly ITourPlanTransfersSettings _settings;

        public ExoToursTransferSearch(HttpClient httpClient, ISerializer serializer, ILogger<TourPlanTransfersSearchBase> logger,
            ILocationManagerService locationManagerService) : base(httpClient, serializer, logger, locationManagerService)
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _locationManagerService = Ensure.IsNotNull(locationManagerService, nameof(locationManagerService));
        }
        public override List<string> SplitDescription(string description)
        {
            var separators = new string[] { " by", " by " };
            var listOfLocations = new List<string>();

            var locations = description.Split(" - ");

            if (locations.Length == 2)
            {
                listOfLocations.Add(locations[0]);
                listOfLocations.Add(locations[1].Split(separators, StringSplitOptions.RemoveEmptyEntries)[0]);
            }

            return listOfLocations;
        }

    }
}
