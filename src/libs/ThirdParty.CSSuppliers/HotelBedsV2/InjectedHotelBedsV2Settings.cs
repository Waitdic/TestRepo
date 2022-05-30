namespace ThirdParty.CSSuppliers.HotelBedsV2
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedHotelBedsV2Settings : SettingsBase, IHotelBedsV2Settings
    {
        protected override string Source => ThirdParties.HOTELBEDSV2;

        public bool AllowAtHotelPayments(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowAtHotelPayments", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int BatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BatchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public string CheckRatesURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CheckRatesURL", tpAttributeSearch);
        }

        public string ContentLanguage(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ContentLanguage", tpAttributeSearch);
        }

        public string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CustomerCountryCode", tpAttributeSearch);
        }

        public bool EnableHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EnableHotelSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch).ToSafeBoolean();
        }

        public bool HotelPackage(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelPackage", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelSearchLimit", tpAttributeSearch).ToSafeInt();
        }

        public int LanguageID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageID", tpAttributeSearch).ToSafeInt();
        }

        public bool NonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("NonRefundableRates", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool Packaging(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Packaging", tpAttributeSearch).ToSafeBoolean();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string SecureBookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SecureBookingURL", tpAttributeSearch);
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }
    }
}