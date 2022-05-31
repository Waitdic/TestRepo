namespace ThirdParty.CSSuppliers.SunHotels
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Search.Settings;
    using ThirdParty.Support;

    public class InjectedSunHotelsSettings : SettingsBase, ISunHotelsSettings
    {
        protected override string Source => ThirdParties.SUNHOTELS;

        public string get_Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string get_Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }

        public string get_EmailAddress(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EmailAddress", tpAttributeSearch);
        }

        public string get_Language(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Language", tpAttributeSearch);
        }

        public string get_Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_SupplierReference(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SupplierReference", tpAttributeSearch);
        }

        public bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string get_BookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookURL", tpAttributeSearch);
        }

        public string get_CancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelURL", tpAttributeSearch);
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandotory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string get_Nationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Nationality", tpAttributeSearch);
        }

        public string get_PreBookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PreBookURL", tpAttributeSearch);
        }

        public string get_AccommodationTypes(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AccommodationTypes", tpAttributeSearch);
        }

        public bool get_RequestPackageRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestPackageRates", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_HotelRequestLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelRequestLimit", tpAttributeSearch).ToSafeInt();
        }
    }
}