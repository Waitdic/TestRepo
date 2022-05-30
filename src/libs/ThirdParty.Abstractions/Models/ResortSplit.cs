namespace ThirdParty.Models
{
    using System.Collections.Generic;
    using iVector.Search.Property;

    /// <summary>
    /// A resort split used for third party searching
    /// </summary>
    public class ResortSplit : IResortSplit
    {
        /// <summary>
        /// Gets or sets the third party supplier.
        /// </summary>
        /// <value>
        /// The third party supplier.
        /// </value>
        public string ThirdPartySupplier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the resort code.
        /// </summary>
        /// <value>
        /// The resort code.
        /// </value>
        public string ResortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hotels.
        /// </summary>
        /// <value>
        /// The hotels.
        /// </value>
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
