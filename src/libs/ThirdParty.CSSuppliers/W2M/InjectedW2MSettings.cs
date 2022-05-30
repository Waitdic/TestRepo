namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedW2MSettings : SettingsBase, IW2MSettings
    {
        protected override string Source => ThirdParties.W2M;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string BookUrl(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookUrl", tpAttributeSearch);
        }

        public string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelURL", tpAttributeSearch);
        }

        public string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultNationality", tpAttributeSearch);
        }

        public bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNonRefundable", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelSearchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string LangID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LangID", tpAttributeSearch).ToLower();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string PreBookUrl(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PreBookUrl", tpAttributeSearch);
        }

        public string SearchUrl(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchUrl", tpAttributeSearch);
        }

        public string SoapActionPrefix(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SoapActionPrefix", tpAttributeSearch);
        }

        public bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SplitMultiroom", tpAttributeSearch).ToSafeBoolean();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }
    }
}
