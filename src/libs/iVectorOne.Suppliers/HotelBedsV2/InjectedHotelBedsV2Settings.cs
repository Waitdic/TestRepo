namespace iVectorOne.CSSuppliers.HotelBedsV2
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

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

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
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

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
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

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }
    }
}