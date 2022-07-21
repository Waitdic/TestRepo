namespace iVectorOne.CSSuppliers.Serhs
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedSerhsSettings : SettingsBase, ISerhsSettings
    {
        protected override string Source => ThirdParties.SERHS;

        public string Branch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Branch", tpAttributeSearch);
        }

        public string ClientCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientCode", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public string TradingGroup(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("TradingGroup", tpAttributeSearch);
        }

        public string CancellationName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationName", tpAttributeSearch);
        }

        public string CancellationEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationEmail", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
    }

}
