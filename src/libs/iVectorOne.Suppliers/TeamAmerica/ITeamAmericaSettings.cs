namespace iVectorOne.CSSuppliers.TeamAmerica
{
    using iVectorOne;
    public interface ITeamAmericaSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string CompanyName(IThirdPartyAttributeSearch tpAttributeSearch);
        string CompanyAddressEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultNationalityCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
