namespace ThirdParty.CSSuppliers.NetStorming
{
    using ThirdParty;

    public interface INetstormingSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Actor(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string ReportingEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        bool SendThreadedRequests(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}