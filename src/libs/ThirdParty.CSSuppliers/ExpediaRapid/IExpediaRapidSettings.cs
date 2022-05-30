namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    public interface IExpediaRapidSettings
    {
        string get_ApiKey(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Secret(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Scheme(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Host(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SearchPath(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_SearchRequestBatchSize(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CountryCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SalesEnvironment(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SortType(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_RatePlanCount(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_PaymentTerms(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_PartnerPointOfSale(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_UserAgent(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_BillingTerms(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_PlatformName(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_RateOption(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_ValidateAffiliateID(IThirdPartyAttributeSearch tpAttributeSearch);
    }

}