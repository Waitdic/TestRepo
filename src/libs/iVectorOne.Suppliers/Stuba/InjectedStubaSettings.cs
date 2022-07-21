namespace iVectorOne.Suppliers.Stuba
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedStubaSettings : SettingsBase, IStubaSettings
    {
        protected override string Source => ThirdParties.STUBA;

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string Organisation(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Organisation", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch).ToSafeBoolean();
        }

        public bool ExcludeUnknownCancellationPolicys(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeUnknownCancellationPolicys", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
    }
}