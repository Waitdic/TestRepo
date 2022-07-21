namespace ThirdParty.Models
{
    /// <summary>
    /// An address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <value>
        /// The house number.
        /// </value>
        public string HouseNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address1.
        /// </summary>
        /// <value>
        /// The address1.
        /// </value>
        public string Address1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        /// <value>
        /// The address2.
        /// </value>
        public string Address2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the town city.
        /// </summary>
        /// <value>
        /// The town city.
        /// </value>
        public string TownCity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the county.
        /// </summary>
        /// <value>
        /// The county.
        /// </value>
        public string County { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        /// <value>
        /// The postcode.
        /// </value>
        public string Postcode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the geography level 1 identifier.
        /// </summary>
        /// <value>
        /// The geography level 1 identifier.
        /// </value>
        public int Geographylevel1ID { get; set; }
    }
}
