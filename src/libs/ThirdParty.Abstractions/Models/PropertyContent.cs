namespace ThirdParty.Models
{
    /// <summary>
    ///   <para>Property Content class, contains information about a class in the property Table</para>
    /// </summary>
    public class PropertyContent
    {
        /// <summary>Gets or sets the property identifier.</summary>
        /// <value>The property identifier.</value>
        public int PropertyID { get; set; }

        /// <summary>Gets or sets the central property identifier.</summary>
        /// <value>The central property identifier.</value>
        public int CentralPropertyID { get; set; }

        /// <summary>Gets or sets the third party key.</summary>
        /// <value>The third party key.</value>
        public string TPKey { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;
    }
}