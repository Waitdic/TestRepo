namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Transfer;
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;

    public abstract class TourPlanTransfersBase : IThirdParty, ISingleSource
    {
        public abstract string Source { get; }

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
