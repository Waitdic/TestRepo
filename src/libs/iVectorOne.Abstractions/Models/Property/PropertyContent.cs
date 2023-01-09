namespace iVectorOne.Models.Property
{
    /// <summary>
    ///   <para>Property Content class, contains information about a class in the property Table</para>
    /// </summary>
    public class PropertyContent
    {
        /// <summary>Gets or sets the property identifier.</summary>
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the central property identifier.</summary>
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets the third party key.</summary>
        public string TPKey { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the property name.</summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>Gets or sets the geography code.</summary>
        public string GeographyCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier identifier.</summary>
        public int SupplierID { get; set; }

        /// <summary>Gets or sets the booking identifier.</summary>
        public int BookingID { get; set; }
    }
}