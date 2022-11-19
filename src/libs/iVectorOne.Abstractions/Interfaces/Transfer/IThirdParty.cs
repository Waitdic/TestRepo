namespace iVectorOne.Transfer
{
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;

    /// <summary>
    /// Defines a third party transfer
    /// </summary>
    public interface IThirdParty
    {
        /// <summary>
        /// Gets the supports live cancellation.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="source">The source.</param>
        /// <returns>A Boolean</returns>
        bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source);

        /// <summary>
        /// Returns success if the pre book succeeds.
        /// </summary>
        /// <param name="transferDetails">The transfer details.</param>
        /// <returns>A boolean</returns>
        Task<bool> PreBookAsync(TransferDetails transferDetails);

        /// <summary>
        /// Books the specified transfer details.
        /// </summary>
        /// <param name="transferDetails">The transfer details.</param>
        /// <returns>the booking reference</returns>
        Task<string> BookAsync(TransferDetails transferDetails);

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="transferDetails">The transfer details.</param>
        /// <returns>a third party cancellation response</returns>
        Task<ThirdPartyCancellationResponse> CancelBookingAsync(TransferDetails transferDetails);

        /// <summary>
        /// Gets the cancellation cost.
        /// </summary>
        /// <param name="transferDetails">The transfer details.</param>
        /// <returns>A third party cancellation fee result</returns>
        Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(TransferDetails transferDetails);
    }
}