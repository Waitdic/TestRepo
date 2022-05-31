namespace ThirdParty.CSSuppliers.MTS
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public  class InjectedMTSSettings : SettingsBase, IMTSSettings
    {
        protected override string Source => ThirdParties.MTS;

        public string ID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ID", tpAttributeSearch);
        }

        public int Type(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return (Get_Value("Type", tpAttributeSearch)).ToSafeInt();
        }

        public string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BaseURL", tpAttributeSearch);
        }

        public int Unique_ID_Type(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return (Get_Value("Unique_ID_Type", tpAttributeSearch)).ToSafeInt();
        }

        public string OverRideID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OverRideID", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return (Get_Value("AllowCancellations", tpAttributeSearch)).ToSafeBoolean();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return (Get_Value("UseGZIP", tpAttributeSearch)).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return (Get_Value("OffsetCancellationDays", tpAttributeSearch)).ToSafeInt();
        }

        public int AuthenticationType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return (Get_Value("AuthenticationType", tpAttributeSearch)).ToSafeInt();
        }

        public string MessagePassword(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MessagePassword", tpAttributeSearch);
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