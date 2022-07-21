namespace iVectorOne.Suppliers
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedYalagoSettings : SettingsBase, IYalagoSettings
    {
        protected override string Source => ThirdParties.YALAGO;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string APIKey(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("APIKey", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SourceMarket", tpAttributeSearch);
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch).ToSafeBoolean();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookURL", tpAttributeSearch);
        }

        public string PreCancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PreCancellationURL", tpAttributeSearch);
        }

        public bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ReturnOpaqueRates", tpAttributeSearch).ToSafeBoolean();
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}