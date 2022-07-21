namespace ThirdParty.CSSuppliers.SunHotels
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedSunHotelsSettings : SettingsBase, ISunHotelsSettings
    {
        protected override string Source => ThirdParties.SUNHOTELS;

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string ContactEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ContactEmail", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string SupplierReference(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SupplierReference", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandotory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CustomerCountryCode", tpAttributeSearch);
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookURL", tpAttributeSearch);
        }

        public string AccommodationTypes(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AccommodationTypes", tpAttributeSearch);
        }

        public bool RequestPackageRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestPackageRates", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
        }
    }
}