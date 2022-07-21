namespace iVectorOne.Search.Settings
{
    using System.Collections.Generic;

    /// <summary>
    /// A class defining the third party configuration
    /// </summary>
    public class ThirdPartyConfiguration
    {
        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        /// <value>
        /// The supplier.
        /// </value>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the settings.</summary>
        /// <value>The settings.</value>
        public Dictionary<string, string> Configurations { get; set; } = new();
    }
}