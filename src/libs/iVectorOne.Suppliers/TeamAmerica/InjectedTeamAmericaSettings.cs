namespace iVectorOne.CSSuppliers.TeamAmerica
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedTeamAmericaSettings : SettingsBase, ITeamAmericaSettings
    {
        protected override string Source => ThirdParties.TEAMAMERICA;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string CompanyAddressEmail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CompanyAddressEmail", tpAttributeSearch);
        }

        public string CompanyName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CompanyName", tpAttributeSearch);
        }

        public string DefaultNationalityCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultNationalityCode", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }
    }
}
