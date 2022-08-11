namespace iVectorOne.Suppliers.BedsWithEase
{
    using iVectorOne;

    public interface IBedsWithEaseSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPStart(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPAvailableHotels(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPHotelReservation(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPCancellationInfo(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPCancelBooking(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPReservationConfirmation(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPEndSession(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgencyAddressLine1(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgencyAddressLine2(IThirdPartyAttributeSearch tpAttributeSearch);
        string OperatorCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
