namespace ThirdParty.CSSuppliers.ChannelManager
{
    public interface IChannelManagerSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        int BrandCode(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}