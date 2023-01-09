namespace iVectorOne.SDK.V2.ExtraSearch
{
    /// <summary>
    /// A class representing a single extra result
    /// </summary>
    public class ExtraResult
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token.
        /// </value>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier reference.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tp session id.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string TPSessionID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extra name.
        /// </summary>
        /// <value>
        /// The extra name.
        /// </value>
        public string ExtraName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extra category.
        /// </summary>
        /// <value>
        /// The extra category.
        /// </value>
        public string ExtraCategory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the use date.
        /// </summary>
        /// <value>
        /// The use date.
        /// </value>
        public string UseDate { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public string EndDate { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the use date
        /// </summary>
        public string UseTime { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the end time
        /// </summary>
        public string EndTime { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the buying channel cost.
        /// </summary>
        public decimal BuyingChannelCost { get; set; }

        /// <summary>
        /// Gets or sets the additional details.
        /// </summary>
        public string AdditionalDetails { get; set; } = string.Empty;

    }
}