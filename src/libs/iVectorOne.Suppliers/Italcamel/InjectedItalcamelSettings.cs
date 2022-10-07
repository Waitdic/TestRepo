namespace iVectorOne.Suppliers.Italcamel
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedItalcamelSettings : SettingsBase, IItalcamelSettings
    {
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageID", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string Login(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Login", tpAttributeSearch);
        }

        protected override string Source => ThirdParties.ITALCAMEL;
    }
}

