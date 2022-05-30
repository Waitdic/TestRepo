namespace ThirdParty
{
    /// <summary>
    /// An adjustment
    /// </summary>
    public class Adjustment
    {
        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>
        /// The total amount.
        /// </value>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the type of the adjustment.
        /// </summary>
        /// <value>
        /// The type of the adjustment.
        /// </value>
        public string AdjustmentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the adjustment.
        /// </summary>
        /// <value>
        /// The name of the adjustment.
        /// </value>
        public string AdjustmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the adjustment amount.
        /// </summary>
        /// <value>
        /// The adjustment amount.
        /// </value>
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the adjustment identifier.
        /// </summary>
        /// <value>
        /// The adjustment identifier.
        /// </value>
        public int AdjustmentID { get; set; }

        /// <summary>
        /// Gets or sets the type of the offer.
        /// </summary>
        /// <value>
        /// The type of the offer.
        /// </value>
        public string OfferType { get; set; } = string.Empty;
    }
}