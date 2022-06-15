namespace ThirdParty.CSSuppliers.iVectorChannelManager
{
    public interface IChannelManagerSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Login(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        int BrandID(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}