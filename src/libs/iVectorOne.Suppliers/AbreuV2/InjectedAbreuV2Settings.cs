namespace iVectorOne.Suppliers.AbreuV2
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedAbreuV2Settings : SettingsBase, IAbreuV2Settings
    {
        protected override string Source => ThirdParties.ABREUV2;

        public string SearchHotelAvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchHotelAvailabilityURL", tpAttributeSearch);
        }
        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }
        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }
        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }
        public string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageID", tpAttributeSearch);
        }
        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }
        public string DatabaseName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DatabaseName", tpAttributeSearch);
        }
        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
        public string Target(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Target", tpAttributeSearch);
        }
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }
        public string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch);
        }
    }
}
