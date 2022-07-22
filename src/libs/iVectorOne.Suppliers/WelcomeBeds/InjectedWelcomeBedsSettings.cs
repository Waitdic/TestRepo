namespace iVectorOne.CSSuppliers
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedWelcomeBedsSettings : SettingsBase, IWelcomeBedsSettings
    {
        protected override string Source => ThirdParties.WELCOMEBEDS;

        public string AccountCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AccountCode", tpAttributeSearch);
        }

        public string AgencyName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgencyName", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string ConnectionString(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ConnectionString", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public bool RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequiresVCard", tpAttributeSearch).ToSafeBoolean();
        }

        public int ResortSearchCap(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ResortSearchCap", tpAttributeSearch).ToSafeInt();
        }

        public int ResortSearchSap(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ResortSearchSap", tpAttributeSearch).ToSafeInt();
        }

        public string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SalesChannel", tpAttributeSearch);
        }

        public string System(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("System", tpAttributeSearch);
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch);
        }
    }
}
