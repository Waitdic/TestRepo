namespace ThirdParty
{
    using System.Collections.Generic;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// An interface for a third party attribute search
    /// </summary>
    public interface IThirdPartyAttributeSearch
    {
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; }
    }
}