namespace iVectorOne.Suppliers.OceanBeds
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedOceanBedsSettings : SettingsBase, IOceanBedsSettings
    {
        protected override string Source => ThirdParties.OCEANBEDS;

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("Password", tpAttributeSearch);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("Currency", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("SearchURL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string ContactPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("ContactPhoneNumber", tpAttributeSearch);
        }

        public string ContactEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("ContactEmail", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationPolicyURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("CancellationPolicyURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("CancellationURL", tpAttributeSearch);
        }
    }
}