namespace iVectorOne.Models
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
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the settings.</summary>
        public Dictionary<string, string> Configurations { get; set; } = new();
    }
}