namespace ThirdParty.CSSuppliers.Acerooms
{
    using ThirdParty;

    public interface IAceroomsSettings
    {
        string APIKey(IThirdPartyAttributeSearch tpAttributeSearch);
        string SecretKey(IThirdPartyAttributeSearch tpAttributeSearch);
        string Signature(IThirdPartyAttributeSearch tpAttributeSearch);
        string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int BatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
