namespace ThirdParty.CSSuppliers.Juniper
{
    public interface IJuniperECTravelSettings : IJuniperBaseSettings
    {
    }

    public interface IJuniperElevateSettings : IJuniperBaseSettings
    {
    }

    public interface IJuniperFastPayHotelsSettings : IJuniperBaseSettings
    {
    }

    public interface IJuniperBaseSettings
    {
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelBookingRuleSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelBookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelCancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelAvailURLSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string PaxCountry(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaxHotelsPerSearchRequest(IThirdPartyAttributeSearch tpAttributeSearch);
        bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ShowCatalogueData(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeNonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelAvailURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelBookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelBookingRuleURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelCancelURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentDutyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
