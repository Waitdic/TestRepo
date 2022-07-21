namespace ThirdParty.CSSuppliers.Juniper
{
    public interface IJuniperSettings
    {
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelBookingRuleSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelBookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelCancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelAvailURLSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string PaxCountry(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int MaxHotelsPerSearchRequest(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool ShowCatalogueData(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool ExcludeNonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelAvailURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelBookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelBookingRuleURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelCancelURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentDutyCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}