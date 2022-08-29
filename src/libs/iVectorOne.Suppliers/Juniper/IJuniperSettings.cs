namespace iVectorOne.Suppliers.Juniper
{
    public interface IJuniperSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string OperatorCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SOAPHotelReservation(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SOAPReservationConfirmation(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SOAPCancelBooking(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SOAPAvailableHotels(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool EnableMultiRoomSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool ShowCatalogueData(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}