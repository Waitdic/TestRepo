namespace iVectorOne.Suppliers.Hotelston
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedHotelstonSettings : SettingsBase, IHotelstonSettings
    {
        protected override string Source => ThirdParties.HOTELSTON;

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public string Email(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Email", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string NetRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("NetRates", tpAttributeSearch);
        }

        public string EndPointUrl(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EndPointUrl", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string SoapRequestUrl(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SoapRequestUrl", tpAttributeSearch);
        }

        public string SoapTypesUrl(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SoapTypesUrl", tpAttributeSearch);
        }

        public bool AlwaysUseDefaultEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AlwaysUseDefaultEmail", tpAttributeSearch).ToSafeBoolean();
        }

        public string DefaultFirstName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultFirstName", tpAttributeSearch);
        }

        public string DefaultLastName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultLastName", tpAttributeSearch);
        }

        public string DefaultPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultPhoneNumber", tpAttributeSearch);
        }
    }
}
