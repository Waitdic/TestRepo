namespace ThirdParty.CSSuppliers.Altura
{
    using ThirdParty;
    public interface IAlturaSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgencyId(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string ExternalId(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        bool SplitMultiRoom(IThirdPartyAttributeSearch tpAttributeSearch);
        string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultCurrency(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
