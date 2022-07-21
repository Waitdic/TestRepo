namespace ThirdParty.CSSuppliers.JonView
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Search.Settings;
    using ThirdParty.Support;

    public class InjectedJonViewSettings : SettingsBase, IJonViewSettings
    {
        protected override string Source => ThirdParties.JONVIEW;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string ClientLoc(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientLoc", tpAttributeSearch);
        }
    }
}