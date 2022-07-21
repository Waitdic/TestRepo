namespace ThirdParty.CSSuppliers.Miki
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedMikiSettings : SettingsBase, IMikiSettings
    {
        protected override string Source => ThirdParties.MIKI;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BaseURL", tpAttributeSearch);
        }

        public string BookingCountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingCountryCode", tpAttributeSearch);
        }

        public string Language(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Language", tpAttributeSearch);
        }

        public string AgentCodeUSD(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentCodeUSD", tpAttributeSearch);
        }

        public string AgentCodeEUR(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentCodeEUR", tpAttributeSearch);
        }

        public string AgentCodeGBP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentCodeGBP", tpAttributeSearch);
        }

        public string AccessCodesFilename(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AccessCodesFilename", tpAttributeSearch);
        }
    }
}
