namespace iVectorOne.Models
{
    /// <summary>
    /// An address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        public string HouseNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address1.
        /// </summary>
        public string Address1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        public string Address2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the town city.
        /// </summary>
        public string TownCity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the county.
        /// </summary>
        public string County { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        public string Postcode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the geography level 1 identifier.
        /// </summary>
        public int Geographylevel1ID { get; set; }
    }
}