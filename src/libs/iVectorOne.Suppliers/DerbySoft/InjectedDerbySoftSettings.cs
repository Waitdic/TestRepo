namespace iVectorOne.Suppliers.DerbySoft
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Support;

    public class InjectedDerbySoftSettings : SettingsBase, IDerbySoftSettings
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

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
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

        public int SecondaryHotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondaryHotelBatchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public string SecondaryPassword(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondaryPassword", tpAttributeSearch, source);
        }

        public string SecondarySearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondarySearchURL", tpAttributeSearch, source);
        }

        public string SupplierID(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SupplierID", tpAttributeSearch, source);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseGZip", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("User", tpAttributeSearch, source);
        }

        public bool EnableUtilitySearch(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("EnableUtilitySearch", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string DefaultMealPlan(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source)
        {
            return Get_Value("DefaultMealPlan", thirdPartyAttributeSearch, source);
        }
    }
}