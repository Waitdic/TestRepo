namespace iVectorOne.Suppliers.PremierInn
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedPremierInnSettings : SettingsBase, IPremierInnSettings
    {
        protected override string Source => ThirdParties.PREMIERINN;

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public bool AllowOnRequest(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowOnRequest", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
