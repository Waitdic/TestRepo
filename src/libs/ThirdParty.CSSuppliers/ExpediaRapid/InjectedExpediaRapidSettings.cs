namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Search.Settings;
    using ThirdParty.Support;

    public class InjectedExpediaRapidSettings : SettingsBase, IExpediaRapidSettings
    {
        protected override string Source => ThirdParties.EXPEDIARAPID;

        public string get_ApiKey(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ApiKey", tpAttributeSearch);
        }

        public string get_Secret(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Secret", tpAttributeSearch);
        }

        public string get_Scheme(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Scheme", tpAttributeSearch);
        }

        public string get_Host(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Host", tpAttributeSearch);
        }

        public string get_SearchPath(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchPath", tpAttributeSearch);
        }

        public string get_PaymentTerms(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PaymentTerms", tpAttributeSearch);
        }

        public string get_PartnerPointOfSale(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PartnerPointOfSale", tpAttributeSearch);
        }

        public string get_LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public int get_SearchRequestBatchSize(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchRequestBatchSize", tpAttributeSearch).ToSafeInt();
        }

        public string get_CountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CountryCode", tpAttributeSearch);
        }

        public string get_SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SalesChannel", tpAttributeSearch);
        }

        public string get_SalesEnvironment(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SalesEnvironment", tpAttributeSearch);
        }

        public string get_SortType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SortType", tpAttributeSearch);
        }

        public int get_RatePlanCount(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RatePlanCount", tpAttributeSearch).ToSafeInt();
        }

        public bool get_UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string get_UserAgent(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserAgent", tpAttributeSearch);
        }
        public string get_BillingTerms(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string get_PlatformName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PlatformName", tpAttributeSearch);
        }

        public string get_RateOption(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RateOption", tpAttributeSearch);
        }

        public bool get_ValidateAffiliateID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ValidateAffiliateID", tpAttributeSearch).ToSafeBoolean();
        }

    }

}