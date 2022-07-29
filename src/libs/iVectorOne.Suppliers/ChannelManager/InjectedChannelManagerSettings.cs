namespace iVectorOne.Suppliers.ChannelManager
{
    using iVectorOne;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Support;
    using iVectorOne.Constants;

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