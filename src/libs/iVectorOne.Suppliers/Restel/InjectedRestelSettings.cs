namespace iVectorOne.Suppliers.Restel
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedRestelSettings : SettingsBase, IRestelSettings
    {
        protected override string Source => ThirdParties.RESTEL;

        public string UserAgent(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserAgent", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string AccessToken(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AccessToken", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public bool GuestNamesAvailable(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GuestNamesAvailable", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
    }
}
