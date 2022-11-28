namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Transfer;
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;
    using Intuitive;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;

    public abstract class TourPlanTransfersBase : IThirdParty, ISingleSource
    {
        public abstract string Source { get; }

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;

        public TourPlanTransfersBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public Task<bool> PreBookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> BookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }
    }
}
