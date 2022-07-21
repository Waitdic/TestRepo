namespace iVectorOne.SDK.V2.PropertyContent
{
    /// <summary>Contains all the information about a properties address</summary>
    public class Address
    {
        /// <summary>Gets or sets the first line of the address.</summary>
        /// <value>The address line1.</value>
        public string AddressLine1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the second line of the address.</summary>
        /// <value>The address the second line of the address.</value>
        public string AddressLine2 { get; set; } = string.Empty;

        /// <summary>Gets or sets the town or city.</summary>
        /// <value>The town or city.</value>
        public string TownOrCity { get; set; } = string.Empty;

        /// <summary>Gets or sets the state of the county or state.</summary>
        /// <value>The state of the county or state</value>
        public string CountyOrState { get; set; } = string.Empty;

        /// <summary>Gets or sets the post or zip code.</summary>
        /// <value>The post or zip code.</value>
        public string PostCodeOrZip { get; set; } = string.Empty;

        /// <summary>Gets or sets the telephone.</summary>
        /// <value>The telephone.</value>
        public string Telephone { get; set; } = string.Empty;
    }
}