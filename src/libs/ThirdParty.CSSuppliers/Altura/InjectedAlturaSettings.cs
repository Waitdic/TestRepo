namespace ThirdParty.CSSuppliers.Altura
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedAlturaSettings : SettingsBase, IAlturaSettings
    {
        protected override string Source => ThirdParties.ALTURA;

        public string AgencyId(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgencyID", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string DefaultCurrency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultCurrency", tpAttributeSearch);
        }

        public string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultNationality", tpAttributeSearch);
        }

        public string ExternalId(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExternalID", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SourceMarket", tpAttributeSearch);
        }

        public bool SplitMultiRoom(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SplitMultiRoom", tpAttributeSearch).ToSafeBoolean();
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
