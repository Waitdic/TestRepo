namespace ThirdParty.CSSuppliers.iVectorChannelManager
{
    using ThirdParty;
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Support;
    using ThirdParty.Constants;

    public class InjectedChannelManagerSettings : SettingsBase, IChannelManagerSettings
    {
        protected override string Source => ThirdParties.CHANNELMANAGER;

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string Login(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Login", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public int BrandID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BrandID", tpAttributeSearch).ToSafeInt();
        }
    }
}