namespace iVectorOne.CSSuppliers.ExpediaRapid
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedExpediaRapidSettings : SettingsBase, IExpediaRapidSettings
    {
        protected override string Source => ThirdParties.EXPEDIARAPID;

        public string APIKey(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("APIKey", tpAttributeSearch);
        }

        public string Secret(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Secret", tpAttributeSearch);
        }

        public string Scheme(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Scheme", tpAttributeSearch);
        }

        public string Host(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Host", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string PaymentTerms(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PaymentTerms", tpAttributeSearch);
        }

        public string PartnerPointOfSale(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PartnerPointOfSale", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CountryCode", tpAttributeSearch);
        }

        public string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SalesChannel", tpAttributeSearch);
        }

        public string SalesEnvironment(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SalesEnvironment", tpAttributeSearch);
        }

        public string SortType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SortType", tpAttributeSearch);
        }

        public int RatePlanCount(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RatePlanCount", tpAttributeSearch).ToSafeInt();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string UserAgent(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserAgent", tpAttributeSearch);
        }
        public string BillingTerms(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string PlatformName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PlatformName", tpAttributeSearch);
        }

        public string RateOption(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RateOption", tpAttributeSearch);
        }

        public bool ValidateAffiliateID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ValidateAffiliateID", tpAttributeSearch).ToSafeBoolean();
        }
    }
}