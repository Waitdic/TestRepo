namespace ThirdParty.Results.Models
{
    /// <summary>
    /// The property data
    /// </summary>
    public class PropertyData
    {
        /// <summary>
        /// Gets or sets the central property identifier.
        /// </summary>
        /// <value>
        /// The central property identifier.
        /// </value>
        public int CentralPropertyID { get; set; }

        /// <summary>
        /// Gets or sets the channel manager contract identifier.
        /// </summary>
        /// <value>
        /// The channel manager contract identifier.
        /// </value>
        public int ChannelManagerContractID { get; set; }

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        /// <remarks>
        /// Overloaded - property or third party property identifier
        /// </remarks>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>Gets or sets the third party key.</summary>
        /// <value>The third party key.</value>
        public string TPKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the source.</summary>
        /// <value>The source.</value>
        public string Source { get; set; } = string.Empty;
    }
}
