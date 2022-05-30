namespace ThirdParty.CSSuppliers.Serhs
{
    public interface ISerhsSettings
    {
        string Branch(IThirdPartyAttributeSearch tpAttributeSearch);
        string ClientCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string TradingGroup(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationName(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}