namespace ThirdParty.CSSuppliers.Jumbo
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedJumboSettings : SettingsBase, IJumboSettings
    {
        protected override string Source => ThirdParties.JUMBO;

        public string get_AgencyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgencyCode", tpAttributeSearch);
        }

        public string get_BrandCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BrandCode", tpAttributeSearch);
        }

        public string get_POS(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("POS", tpAttributeSearch);
        }

        public string get_CommonsURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CommonsURL", tpAttributeSearch);
        }

        public string get_HotelBookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBookingURL", tpAttributeSearch);
        }

        public string get_BasketHandlerURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BasketHandlerURL", tpAttributeSearch);
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_Language(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Language", tpAttributeSearch);
        }

        public bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string get_NationalityBasedCredentials(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("NationalityBasedCredentials", tpAttributeSearch);
        }
    }
}