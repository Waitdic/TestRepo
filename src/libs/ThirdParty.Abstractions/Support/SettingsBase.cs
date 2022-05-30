namespace ThirdParty.Support
{
    using System.Linq;

    public abstract class SettingsBase
    {
        protected abstract string Source { get; }

        public string Get_Value(string key, IThirdPartyAttributeSearch search)
        {
            if (!search.ThirdPartyConfigurations.FirstOrDefault(c => c.Supplier == Source).Configurations.TryGetValue(key, out string value))
            {
                value = "";
            }

            return value;
        }
    }
}