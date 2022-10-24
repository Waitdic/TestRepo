namespace iVectorOne.Suppliers.Juniper
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Support;

    public class InjectedJuniperSettings : SettingsBase, IJuniperSettings
    {
        protected override string Source => string.Empty;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("BookingURL", tpAttributeSearch, source);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CancellationURL", tpAttributeSearch, source);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Currency", tpAttributeSearch, source);
        }

        public string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CustomerCountryCode", tpAttributeSearch, source);
        }

        public bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SplitMultiroom", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("GenericURL", tpAttributeSearch, source);
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LanguageCode", tpAttributeSearch, source);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public string OperatorCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OperatorCode", tpAttributeSearch, source);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PrebookURL", tpAttributeSearch, source);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SearchURL", tpAttributeSearch, source);
        }

        public string SOAPAvailableHotels(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SOAPAvailableHotels", tpAttributeSearch, source);
        }

        public string SOAPCancelBooking(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SOAPCancelBooking", tpAttributeSearch, source);
        }

        public string SOAPHotelReservation(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SOAPHotelReservation", tpAttributeSearch, source);
        }

        public string SOAPReservationConfirmation(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentDutyCode", tpAttributeSearch, source);
        }

        public bool ShowCatalogueData(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ShowCatalogueData", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentDutyCode", tpAttributeSearch, source).ToSafeBoolean();
        }
    }
}