namespace iVectorOne.CSSuppliers.OceanBeds
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

        public string SearchEndPoint(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("SearchEndPoint", tpAttributeSearch);
        }

        public bool UseGzip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string Telephone(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("Telephone", tpAttributeSearch);
        }

        public string DefaultEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("DefaultEmail", tpAttributeSearch);
        }

        public string BookEndPoint(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("BookEndPoint", tpAttributeSearch);
        }

        public string GetCancellationEndPoint(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("GetCancellationEndPoint", tpAttributeSearch);
        }

        public string ConfirmCancelEndPoint(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("ConfirmCancelEndPoint", tpAttributeSearch);
        }
    }
}