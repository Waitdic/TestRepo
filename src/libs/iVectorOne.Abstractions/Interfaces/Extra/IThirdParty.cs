namespace iVectorOne.Extra
{
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;

    /// <summary>
    /// Defines a third party transfer
    /// </summary>
    public interface IThirdParty
    {
        /// <summary>
        /// Gets the supports live cancellation.
        /// </summary>
        /// <param name="tpAttributeSearch">The third party search attribute search.</param>
        /// <param name="source">The source.</param>
        /// <returns>A Boolean</returns>
        bool SupportsLiveCancellation(IThirdPartyAttributeSearch tpAttributeSearch, string source);

        /// <summary>
        /// Returns success if the pre book succeeds.
        /// </summary>
        /// <param name="extraDetails">The extra details.</param>
        /// <returns>A boolean</returns>
        Task<bool> PreBookAsync(ExtraDetails extraDetails);

        /// <summary>
        /// Books the specified transfer details.
        /// </summary>
        /// <param name="extraDetails">The extra details.</param>
        /// <returns>the booking reference</returns>
        Task<string> BookAsync(ExtraDetails extraDetails);

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="extraDetails">The extra details.</param>
        /// <returns>a third party cancellation response</returns>
        Task<ThirdPartyCancellationResponse> CancelBookingAsync(ExtraDetails extraDetails);

        /// <summary>
        /// Gets the cancellation cost.
        /// </summary>
        /// <param name="extraDetails">The extra details.</param>
        /// <returns>A third party cancellation fee result</returns>
        Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(ExtraDetails extraDetails);

        /// <summary>
        /// Validate the third party Settings.
        /// </summary>
        /// <param name="extraDetails">The extra details.</param>
        /// <returns>boolean representing if the setting are present or not.</returns>
        bool ValidateSettings(ExtraDetails extraDetails);
    }
}