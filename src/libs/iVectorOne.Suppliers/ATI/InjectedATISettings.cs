namespace iVectorOne.Suppliers.ATI
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedATISettings : SettingsBase, IATISettings
    {
        protected override string Source => ThirdParties.ATI;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
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

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("Version", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch).ToSafeBoolean();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
