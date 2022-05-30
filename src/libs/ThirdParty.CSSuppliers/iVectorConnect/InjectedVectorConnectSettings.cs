namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Support;
    using ThirdParty.Constants;

    public class InjectedBookabedSettings : InjectedIVectorConnectSettings, IBookabedSettings
    {
        protected override string Source => ThirdParties.BOOKABED;
    }

    public class InjectedImperatoreSettings : InjectedIVectorConnectSettings, IImperatoreSettings
    {
        protected override string Source => ThirdParties.IMPERATORE;
    }

    public abstract class InjectedIVectorConnectSettings : SettingsBase, IIVectorConnectSettings
    {
        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool UseAgentDetails(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseAgentDetails", tpAttributeSearch).ToSafeBoolean();
        }

        public string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentAddress", tpAttributeSearch);
        }

        public string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentEmailAddress", tpAttributeSearch);
        }

        public string Login(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Login", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string AgentReference(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentReference", tpAttributeSearch);
        }

        public int SellingCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SellingCurrencyID", tpAttributeSearch).ToSafeInt();
        }
    }
}
