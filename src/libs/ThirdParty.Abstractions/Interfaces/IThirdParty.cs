namespace ThirdParty
{
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    /// <summary>
    /// Defines a third party
    /// </summary>
    public interface IThirdParty
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        string Source { get; }

        /// <summary>
        /// Gets a value indicating whether [supports remarks].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supports remarks]; otherwise, <c>false</c>.
        /// </value>
        bool SupportsRemarks { get; }

        /// <summary>
        /// Gets a value indicating whether [supports booking search].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supports booking search]; otherwise, <c>false</c>.
        /// </value>
        bool SupportsBookingSearch { get; }

        /// <summary>
        /// Gets the supports live cancellation.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="source">The s source.</param>
        /// <returns>A Boolean</returns>
        bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source);

        /// <summary>
        /// Gets the take saving from commission margin.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>A Boolean</returns>
        bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails);

        /// <summary>
        /// Gets the offset cancellation days.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>The number of Offset Cancellation Days</returns>
        int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails);

        /// <summary>
        /// Gets a value indicating whether [requires v card].
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>a boolean</returns>
        /// <value>
        ///   <c>true</c> if [requires v card]; otherwise, <c>false</c>.
        /// </value>
        bool RequiresVCard(VirtualCardInfo info);

        /// <summary>
        /// Returns success if the pre book succeeds.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>A boolean</returns>
        bool PreBook(PropertyDetails propertyDetails);

        /// <summary>
        /// Books the specified property details.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>the booking reference</returns>
        string Book(PropertyDetails propertyDetails);

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>a third party cancellation response</returns>
        ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails);

        /// <summary>
        /// Gets the cancellation cost.
        /// </summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <returns>A third party cancellation fee result</returns>
        ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails);

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