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

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch).ToLower();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookURL", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string SoapActionPrefix(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SoapActionPrefix", tpAttributeSearch);
        }

        public bool EnableMultiRoomSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EnableMultiRoomSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }
    }
}