namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using ThirdParty;

    public interface IBookabedSettings : IIVectorConnectSettings
    {
    }

    public interface IImperatoreSettings : IIVectorConnectSettings
    {
    }

    public interface IIVectorConnectSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseAgentDetails(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch);
        string Login(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentReference(IThirdPartyAttributeSearch tpAttributeSearch);
        int SellingCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
