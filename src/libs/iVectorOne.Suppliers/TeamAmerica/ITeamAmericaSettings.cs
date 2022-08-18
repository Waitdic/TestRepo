namespace iVectorOne.Suppliers.TeamAmerica
{
    using iVectorOne;
    public interface ITeamAmericaSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string UserAgent(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
