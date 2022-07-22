namespace iVectorOne.Models
{
    /// <summary>
    /// A result object for third party booking status updates
    /// </summary>
    public class ThirdPartyBookingStatusUpdateResult
    {
        /// <summary>
        /// The status
        /// </summary>
        private ThirdPartyBookingStatus status = ThirdPartyBookingStatus.OnRequest;

        /// <summary>
        /// The success
        /// </summary>
        private bool success = false;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public ThirdPartyBookingStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ThirdPartyBookingStatusUpdateResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get
            {
                return this.success;
            }

            set
            {
                this.success = value;
            }
        }
    }
}
