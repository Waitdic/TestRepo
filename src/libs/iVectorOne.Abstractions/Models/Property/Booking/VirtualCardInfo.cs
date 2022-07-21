namespace ThirdParty.Models.Property.Booking
{
    using System.Collections.Generic;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// The virtual card info
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class VirtualCardInfo : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets the The third party reference
        /// </summary>
        public string TPReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the The source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; } = new();
    }
}