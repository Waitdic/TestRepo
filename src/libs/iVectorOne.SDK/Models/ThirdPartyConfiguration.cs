namespace iVectorOne.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A class defining the third party configuration
    /// </summary>
    public class ThirdPartyConfiguration
    {
        /// <summary>Gets or sets the supplier.</summary>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier identifier.</summary>
        public int SupplierID { get; set; }

        /// <summary>Gets or sets a value indicating whether search requests should be logged</summary>
        public bool LogSearchRequests { get; set; }

        /// <summary>Gets or sets the settings.</summary>
        public Dictionary<string, string> Configurations { get; set; } = new();
    }
}