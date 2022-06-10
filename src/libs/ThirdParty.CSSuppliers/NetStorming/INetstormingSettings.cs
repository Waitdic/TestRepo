namespace ThirdParty.CSSuppliers.Netstorming
{
    using ThirdParty;

    public interface INetstormingSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Actor(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string User(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string ReportingEmail(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool SendThreadedRequests(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}