namespace iVectorOne.Suppliers.iVectorConnect
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Support;

    public class InjectediVectorConnectSettings : SettingsBase, IiVectorConnectSettings
    {
        protected override string Source => string.Empty;

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("GenericURL", tpAttributeSearch, source);
        }

        public bool UseAgentDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseAgentDetails", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentAddress", tpAttributeSearch, source);
        }

        public string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentEmailAddress", tpAttributeSearch, source);
        }

        public string Login(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Login", tpAttributeSearch, source);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string AgentReference(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentReference", tpAttributeSearch, source);
        }

        public int SellingCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SellingCurrencyID", tpAttributeSearch, source).ToSafeInt();
        }
    }
}