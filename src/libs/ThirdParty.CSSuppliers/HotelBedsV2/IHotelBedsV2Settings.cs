namespace ThirdParty.CSSuppliers.HotelBedsV2
{
    using ThirdParty;

    public interface IHotelBedsV2Settings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);

        string User(IThirdPartyAttributeSearch tpAttributeSearch);

        string Password(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        string ContentLanguage(IThirdPartyAttributeSearch tpAttributeSearch);

        int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch);

        bool NonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch);

        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);

        string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch);

        bool EnableHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch);

        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string CheckRatesURL(IThirdPartyAttributeSearch tpAttributeSearch);

        int BatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);

        string SecureBookingURL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool Packaging(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowAtHotelPayments(IThirdPartyAttributeSearch tpAttributeSearch);

        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch);

        bool HotelPackage(IThirdPartyAttributeSearch tpAttributeSearch);

        int LanguageID(IThirdPartyAttributeSearch tpAttributeSearch);

        bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch);

    }
}