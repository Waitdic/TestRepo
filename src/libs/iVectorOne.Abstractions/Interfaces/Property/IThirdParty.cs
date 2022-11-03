namespace iVectorOne.Interfaces
{
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    /// <summary>
    /// Defines a third party
    /// </summary>
    public interface IThirdParty
    {
        /// <summary>
        /// Gets a value indicating whether [supports remarks].
        /// </summary>
        bool SupportsRemarks { get; }

        /// <summary>
        /// Gets a value indicating whether [supports booking search].
        /// </summary>
        bool SupportsBookingSearch { get; }

        /// <summary>
        /// Gets the supports live cancellation.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="source">The source.</param>
        /// <returns>A Boolean</returns>
        bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source);

        /// <summary>
        /// Gets the offset cancellation days.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="source">The source.</param>
        /// <returns>The number of Offset Cancellation Days</returns>
        int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source);

        /// <summary>
        /// Gets a value indicating whether [requires v card].
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="source">The source.</param>
        /// <returns>a boolean</returns>
        bool RequiresVCard(VirtualCardInfo info, string source);

        /// <summary>
        /// Returns success if the pre book succeeds.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>A boolean</returns>
        Task<bool> PreBookAsync(PropertyDetails propertyDetails);

        /// <summary>
        /// Books the specified property details.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>the booking reference</returns>
        Task<string> BookAsync(PropertyDetails propertyDetails);

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>a third party cancellation response</returns>
        Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails);

        /// <summary>
        /// Gets the cancellation cost.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>A third party cancellation fee result</returns>
        Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails);

        /// <summary>
        /// Bookings the search.
        /// </summary>
        /// <param name="bookingSearchDetails">The o booking search details.</param>
        /// <returns>A third party booking search result</returns>
        ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails);

        /// <summary>
        /// Bookings the status update.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>A third party booking status update result</returns>
        ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails);

        /// <summary>
        /// Creates the reconciliation reference.
        /// </summary>
        /// <param name="inputReference">The s input reference.</param>
        /// <returns>the cancellation reference</returns>
        string CreateReconciliationReference(string inputReference);

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        void EndSession(PropertyDetails propertyDetails);
    }
}