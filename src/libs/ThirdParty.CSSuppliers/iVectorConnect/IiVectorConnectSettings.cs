namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using ThirdParty;

    public interface IiVectorConnectSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseAgentDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Login(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentReference(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int SellingCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}