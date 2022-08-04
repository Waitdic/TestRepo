namespace iVectorOne.Suppliers.FastPayHotels
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedFastPayHotelsSettings : SettingsBase, IFastPayHotelsSettings
    {
        protected override string Source => ThirdParties.FASTPAYHOTELS;

        public string AvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AvailabilityURL", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string ClientId(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientId", tpAttributeSearch);
        }

        public string ClientSecret(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientSecret", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string XMLUser(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("XMLUser", tpAttributeSearch);
        }

        public string UserRateType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserRateType", tpAttributeSearch);
        }
        public string DelimChar(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DelimChar", tpAttributeSearch);
        }

        public string AccessToken(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AccessToken", tpAttributeSearch);
        }
        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }
        public string CountryOfResidence(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CountryOfResidence", tpAttributeSearch);
        }

        public bool UseCurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseCurrencyCode", tpAttributeSearch).ToSafeBoolean();
        }
    }
}