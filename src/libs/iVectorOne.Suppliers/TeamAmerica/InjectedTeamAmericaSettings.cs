namespace iVectorOne.Suppliers.TeamAmerica
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedTeamAmericaSettings : SettingsBase, ITeamAmericaSettings
    {
        protected override string Source => ThirdParties.TEAMAMERICA;

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string UserAgent(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserAgent", tpAttributeSearch);
        }

        public string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentEmailAddress", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
