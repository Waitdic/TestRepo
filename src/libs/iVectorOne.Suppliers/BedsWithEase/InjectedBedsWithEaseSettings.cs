namespace iVectorOne.Suppliers.BedsWithEase
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedBedsWithEaseSettings : SettingsBase, IBedsWithEaseSettings
    {
        protected override string Source => ThirdParties.BEDSWITHEASE;

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public string SOAPStart(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPStart", tpAttributeSearch);
        }

        public string SOAPAvailableHotels(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPAvailableHotels", tpAttributeSearch);
        }

        public string SOAPHotelReservation(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPHotelReservation", tpAttributeSearch);
        }

        public string SOAPCancellationInfo(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPCancellationInfo", tpAttributeSearch);
        }

        public string SOAPCancelBooking(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPCancelBooking", tpAttributeSearch);
        }

        public string SOAPReservationConfirmation(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPReservationConfirmation", tpAttributeSearch);
        }

        public string SOAPEndSession(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SOAPEndSession", tpAttributeSearch);
        }

        public string AgencyAddressLine1(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgencyAddressLine1", tpAttributeSearch);
        }

        public string AgencyAddressLine2(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgencyAddressLine2", tpAttributeSearch);
        }

        public string OperatorCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OperatorCode", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
