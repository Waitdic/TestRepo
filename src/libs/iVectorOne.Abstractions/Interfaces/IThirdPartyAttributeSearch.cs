namespace iVectorOne
{
    using System.Collections.Generic;
    using iVectorOne.Models;

    /// <summary>
    /// An interface for a third party attribute search
    /// </summary>
    public interface IThirdPartyAttributeSearch
    {
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; }
    }
}