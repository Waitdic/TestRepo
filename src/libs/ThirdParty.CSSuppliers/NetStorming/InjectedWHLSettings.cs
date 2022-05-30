namespace ThirdParty.CSSuppliers.NetStorming
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedWHLSettings : SettingsBase, INetstormingSettings
    {
        // if any other classes inherit from netstorming base in the future, this should be split like the Derbysoft settings
        protected override string Source => ThirdParties.WHL;

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string Actor(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Actor", tpAttributeSearch);
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

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string ReportingEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ReportingEmail", tpAttributeSearch);
        }

        public bool SendThreadedRequests(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SendThreadedRequests", tpAttributeSearch).ToSafeBoolean();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultNationality", tpAttributeSearch);
        }
    }
}