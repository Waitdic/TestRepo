namespace iVectorOne.Models
{
    /// <summary>
    /// Represents a Booking Country in the Database
    /// </summary>
    public class BookingCountry
    {
        /// <summary>
        /// Gets or sets the Booking Country identifier.
        /// </summary>
        /// <value>
        /// The currency identifier.
        /// </value>
        public int BookingCountryID { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party Country code.
        /// </summary>
        /// <value>
        /// The third party currency code.
        /// </value>
        public string TPBookingCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Country code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string BookingCountryCode { get; set; } = string.Empty;
    }
}
