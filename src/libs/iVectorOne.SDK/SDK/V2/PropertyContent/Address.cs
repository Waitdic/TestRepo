namespace iVectorOne.SDK.V2.PropertyContent
{
    /// <summary>Contains all the information about a properties address</summary>
    public class Address
    {
        /// <summary>Gets or sets the first line of the address.</summary>
        public string AddressLine1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the second line of the address.</summary>
        public string AddressLine2 { get; set; } = string.Empty;

        /// <summary>Gets or sets the town or city.</summary>
        public string TownOrCity { get; set; } = string.Empty;

        /// <summary>Gets or sets the state of the county or state.</summary>
        public string CountyOrState { get; set; } = string.Empty;

        /// <summary>Gets or sets the post or zip code.</summary>
        public string PostCodeOrZip { get; set; } = string.Empty;

        /// <summary>Gets or sets the telephone.</summary>
        public string Telephone { get; set; } = string.Empty;
    }
}