namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedYalagoSettings : SettingsBase, IYalagoSettings
    {
        protected override string Source => ThirdParties.YALAGO;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string API_Key(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("API_Key", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelURL", tpAttributeSearch);
        }

        public string CountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CountryCode", tpAttributeSearch);
        }

        public bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNonRefundable", tpAttributeSearch).ToSafeBoolean();
        }

        public string Language(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Language", tpAttributeSearch);
        }

        public string PreBookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PreBookURL", tpAttributeSearch);
        }

        public string PreCancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PreCancelURL", tpAttributeSearch);
        }

        public bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ReturnOpaqueRates", tpAttributeSearch).ToSafeBoolean();
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
