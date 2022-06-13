namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Support;

    public class InjectedDerbySoftSettings : SettingsBase, IDerbySoftSettings
    {
        protected override string Source => string.Empty;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string BookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("BookURL", tpAttributeSearch, source);
        }

        public string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CancelURL", tpAttributeSearch, source);
        }

        public bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ExcludeNonRefundable", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelSearchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string PreBookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PreBookURL", tpAttributeSearch, source);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SearchURL", tpAttributeSearch, source);
        }

        public int ShoppingEngineHotelsBatchSize(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ShoppingEngineHotelsBatchSize", tpAttributeSearch, source).ToSafeInt();
        }

        public string ShoppingEnginePassword(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ShoppingEnginePassword", tpAttributeSearch, source);
        }

        public string ShoppingEngineURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ShoppingEngineURL", tpAttributeSearch, source);
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

        public bool UseShoppingEngineForSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseShoppingEngineForSearch", tpAttributeSearch, source).ToSafeBoolean();
        }
    }
}