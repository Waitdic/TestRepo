namespace iVectorOne.Suppliers.GowaySydneyTransfers
{
    using Intuitive;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using System;
    using System.Net.Http;

    public class GowaySydneyTransfersSearch : TourPlanTransfersSearchBase
    {
        public override string Source => ThirdParties.GOWAYSYDNEYTRANSFERS;

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;

        public GowaySydneyTransfersSearch(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient) : base(settings, httpClient)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
        }
    }
}
