namespace ThirdParty.CSSuppliers.BedsWithEase
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedBedsWithEaseSettings : SettingsBase, IBedsWithEaseSettings
    {
        protected override string Source => ThirdParties.BEDSWITHEASE;

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
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
