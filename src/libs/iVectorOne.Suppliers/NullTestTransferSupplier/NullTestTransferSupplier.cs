namespace iVectorOne.Suppliers.NullTestTransferSupplier
{
    using iVectorOne.Interfaces;
    using iVectorOne.Models.Property;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Transfer;
    using System.Threading.Tasks;

    public partial class NullTestTransferSupplier : IThirdParty, ISingleSource
    {
        public string Source => throw new System.NotImplementedException();

        Task<string> IThirdParty.BookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        Task<ThirdPartyCancellationResponse> IThirdParty.CancelBookingAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        Task<ThirdPartyCancellationFeeResult> IThirdParty.GetCancellationCostAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        Task<bool> IThirdParty.PreBookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }
    }
}
