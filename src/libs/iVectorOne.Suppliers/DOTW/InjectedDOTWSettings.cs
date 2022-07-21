namespace iVectorOne.Suppliers.DOTW
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedDOTWSettings : SettingsBase, IDOTWSettings
    {
        protected override string Source => ThirdParties.DOTW;

        public string CompanyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CompanyCode", tpAttributeSearch);
        }

        public string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CustomerCountryCode", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }

        public int DefaultCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultCurrencyID", tpAttributeSearch).ToSafeInt();
        }

        public bool ExcludeDOTWThirdParties(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeDOTWThirdParties", tpAttributeSearch).ToSafeBoolean();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string RequestCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestCurrencyID", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string ThreadedSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ThreadedSearch", tpAttributeSearch);
        }

        public bool UseMinimumSellingPrice(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseMinimumSellingPrice", tpAttributeSearch).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public int Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch).ToSafeInt();
        }
    }
}