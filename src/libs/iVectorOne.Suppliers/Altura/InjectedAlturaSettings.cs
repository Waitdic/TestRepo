namespace iVectorOne.CSSuppliers.Altura
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

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

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
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

        public bool EnableMultiRoomSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EnableMultiRoomSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
