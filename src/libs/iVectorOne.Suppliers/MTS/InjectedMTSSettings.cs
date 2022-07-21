namespace iVectorOne.CSSuppliers.MTS
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public  class InjectedMTSSettings : SettingsBase, IMTSSettings
    {
        protected override string Source => ThirdParties.MTS;

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public int Type(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Type", tpAttributeSearch).ToSafeInt();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public int Unique_ID_Type(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Unique_ID_Type", tpAttributeSearch).ToSafeInt();
        }

        public string OverRideID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OverRideID", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public int AuthenticationType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AuthenticationType", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string ID_Context(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ID_Context", tpAttributeSearch);
        }

        public string OverrideCountries(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OverrideCountries", tpAttributeSearch);
        }

        public string AuthenticationID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AuthenticationID", tpAttributeSearch);
        }

        public string Instance(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Instance", tpAttributeSearch);
        }
    }
}