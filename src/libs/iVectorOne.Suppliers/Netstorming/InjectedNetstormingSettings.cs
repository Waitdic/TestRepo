namespace iVectorOne.CSSuppliers.Netstorming
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Support;

    public class InjectedNetstormingSettings : SettingsBase, INetstormingSettings
    {
        protected override string Source => string.Empty;

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("GenericURL", tpAttributeSearch, source);
        }

        public string Actor(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Actor", tpAttributeSearch, source);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("User", tpAttributeSearch, source);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Version", tpAttributeSearch, source);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string ContactEmail(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ContactEmail", tpAttributeSearch, source);
        }

        public bool SendThreadedRequests(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SendThreadedRequests", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LanguageCode", tpAttributeSearch, source);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseGZip", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch, source);
        }
    }
}