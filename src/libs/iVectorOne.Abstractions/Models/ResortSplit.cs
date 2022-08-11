namespace iVectorOne.Models
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
        public string ThirdPartySupplier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the resort code.
        /// </summary>
        public string ResortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hotels.
        /// </summary>
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}