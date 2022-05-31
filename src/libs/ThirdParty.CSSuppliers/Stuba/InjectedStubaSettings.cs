namespace ThirdParty.CSSuppliers.Stuba
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Search.Settings;
    using ThirdParty.Support;

    public class InjectedStubaSettings : SettingsBase, IStubaSettings
    {
        protected override string Source => ThirdParties.STUBA;

        public string get_URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public int get_MaxHotelsPerRequest(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaxHotelsPerRequest", tpAttributeSearch).ToSafeInt();
        }

        public string get_Organisation(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Organisation", tpAttributeSearch);
        }

        public string get_Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }

        public string get_Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string get_Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch);
        }

        public string get_Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string get_Nationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Nationality", tpAttributeSearch);
        }

        public bool get_ExcludeNonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNonRefundableRates", tpAttributeSearch).ToSafeBoolean();
        }

        public bool get_ExcludeUnknownCancellationPolicys(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeUnknownCancellationPolicys", tpAttributeSearch).ToSafeBoolean();
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

    }
}