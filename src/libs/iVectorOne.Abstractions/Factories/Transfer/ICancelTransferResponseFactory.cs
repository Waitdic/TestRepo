namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using Precancel = SDK.V2.TransferPrecancel;
    using Cancel = SDK.V2.TransferCancel;
    using iVectorOne.Models.Property;

    /// <summary>
    /// A factory that creates transfer cancellation responses using the provided transfer details
    /// </summary>
    public interface ICancelTransferResponseFactory
    {
        /// <summary>
        /// Creates a cancellation response using information from the transfer details
        /// </summary>
        /// <param name="thirdPartyCancellationResponse">The third party cancellation response returned from the interface</param>
        /// <returns>A book response</returns>
        Cancel.Response Create(ThirdPartyCancellationResponse thirdPartyCancellationResponse);

        /// <summary>
        /// Creates a cancellation fees response using information from the transfer details
        /// </summary>
        /// <param name="fees">The third party cancellation fees returned from the interface</param>
        /// <param name="transferDetails">The Transfer Details</param>
        /// <returns>A book response</returns>
        Task<Precancel.Response> CreateFeesResponseAsync(ThirdPartyCancellationFeeResult fees, TransferDetails transferDetails);
    }
}