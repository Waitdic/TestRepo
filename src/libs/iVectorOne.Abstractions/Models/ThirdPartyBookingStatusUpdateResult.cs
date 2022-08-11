namespace iVectorOne.Models
{
    /// <summary>
    /// A result object for third party booking status updates
    /// </summary>
    public class ThirdPartyBookingStatusUpdateResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public ThirdPartyBookingStatus Status { get; set; } = ThirdPartyBookingStatus.OnRequest;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ThirdPartyBookingStatusUpdateResult"/> is success.
        /// </summary>
        public bool Success { get; set; }
    }
}