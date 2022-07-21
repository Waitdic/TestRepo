namespace iVectorOne.Suppliers.AbreuV2
{
    using iVectorOne;

    public interface IAbreuV2Settings
    {
        string SearchHotelAvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string DatabaseName(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string Target(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}