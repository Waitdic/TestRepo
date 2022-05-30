namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedDerbySoftClubMedSettings : InjectedDerbySoftSettings, IDerbySoftClubMedSettings
    {
        protected override string Source => ThirdParties.DERBYSOFTCLUBMED;
    }

    public class InjectedDerbySoftMarriottSettings : InjectedDerbySoftSettings, IDerbySoftMarriottSettings
    {
        protected override string Source => ThirdParties.DERBYSOFTSMARRIOTT;
    }

    public class InjectedDerbySoftSynxisSettings : InjectedDerbySoftSettings, IDerbySoftSynxisSettings
    {
        protected override string Source => ThirdParties.DERBYSOFTSYNXIS;
    }

    public abstract class InjectedDerbySoftSettings : SettingsBase, IDerbySoftSettings
    {
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string BookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookURL", tpAttributeSearch);
        }

        public string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelURL", tpAttributeSearch);
        }

        public bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNonRefundable", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelSearchLimit", tpAttributeSearch).ToSafeInt();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string PreBookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PreBookURL", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public int ShoppingEngineHotelsBatchSize(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ShoppingEngineHotelsBatchSize", tpAttributeSearch).ToSafeInt();
        }

        public string ShoppingEnginePassword(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ShoppingEnginePassword", tpAttributeSearch);
        }

        public string ShoppingEngineURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ShoppingEngineURL", tpAttributeSearch);
        }

        public string SupplierID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SupplierID", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public bool UseShoppingEngineForSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseShoppingEngineForSearch", tpAttributeSearch).ToSafeBoolean();
        }
    }
}