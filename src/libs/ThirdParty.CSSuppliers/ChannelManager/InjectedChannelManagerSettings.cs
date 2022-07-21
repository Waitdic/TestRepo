namespace ThirdParty.CSSuppliers.ChannelManager
{
    using ThirdParty;
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Support;
    using ThirdParty.Constants;

    public class InjectedChannelManagerSettings : SettingsBase, IChannelManagerSettings
    {
        protected override string Source => ThirdParties.CHANNELMANAGER;

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public int BrandCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BrandCode", tpAttributeSearch).ToSafeInt();
        }
    }
}