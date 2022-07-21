namespace iVectorOne.Support
{
    using System.Linq;

    public abstract class SettingsBase
    {
        protected abstract string Source { get; }

        public string Get_Value(string key, IThirdPartyAttributeSearch search)
            => Get_Value(key, search, Source);

        public string Get_Value(string key, IThirdPartyAttributeSearch search, string source)
        {
            if (!search.ThirdPartyConfigurations.FirstOrDefault(c => c.Supplier == source).Configurations.TryGetValue(key, out string value))
            {
                value = "";
            }

            return value;
        }
    }
}