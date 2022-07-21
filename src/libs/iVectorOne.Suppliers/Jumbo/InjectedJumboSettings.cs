namespace ThirdParty.CSSuppliers.Jumbo
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedJumboSettings : SettingsBase, IJumboSettings
    {
        protected override string Source => ThirdParties.JUMBO;

        public string AgencyID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgencyID", tpAttributeSearch);
        }

        public string BrandCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BrandCode", tpAttributeSearch);
        }

        public string POS(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("POS", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string NationalityBasedCredentials(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("NationalityBasedCredentials", tpAttributeSearch);
        }
    }
}