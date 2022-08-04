namespace iVectorOne.Suppliers.ExpediaRapid
{
    public interface IExpediaRapidSettings
    {
        string APIKey(IThirdPartyAttributeSearch tpAttributeSearch);
        string Secret(IThirdPartyAttributeSearch tpAttributeSearch);
        string Scheme(IThirdPartyAttributeSearch tpAttributeSearch);
        string Host(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
        string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch);
        string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch);
        string SalesEnvironment(IThirdPartyAttributeSearch tpAttributeSearch);
        string SortType(IThirdPartyAttributeSearch tpAttributeSearch);
        int RatePlanCount(IThirdPartyAttributeSearch tpAttributeSearch);
        string PaymentTerms(IThirdPartyAttributeSearch tpAttributeSearch);
        string PartnerPointOfSale(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string UserAgent(IThirdPartyAttributeSearch tpAttributeSearch);
        string BillingTerms(IThirdPartyAttributeSearch tpAttributeSearch);
        string PlatformName(IThirdPartyAttributeSearch tpAttributeSearch);
        string RateOption(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ValidateAffiliateID(IThirdPartyAttributeSearch tpAttributeSearch);
        string TermsAndConditionsLink(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}