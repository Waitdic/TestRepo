namespace iVectorOne.CSSuppliers.SunHotels
{
    public interface ISunHotelsSettings
    {
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string ContactEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string SupplierReference(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandotory);
        string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AccommodationTypes(IThirdPartyAttributeSearch tpAttributeSearch);
        bool RequestPackageRates(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}