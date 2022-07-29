namespace iVectorOne.Suppliers.HotelBedsV2
{
    using iVectorOne;

    public interface IHotelBedsV2Settings
    {
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string User(IThirdPartyAttributeSearch tpAttributeSearch);

        string Password(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);

        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);

        string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch);

        bool EnableHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch);

        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string CheckRatesURL(IThirdPartyAttributeSearch tpAttributeSearch);

        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);

        string SecureBookingURL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool Packaging(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowAtHotelPayments(IThirdPartyAttributeSearch tpAttributeSearch);

        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch);

        bool HotelPackage(IThirdPartyAttributeSearch tpAttributeSearch);

        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}