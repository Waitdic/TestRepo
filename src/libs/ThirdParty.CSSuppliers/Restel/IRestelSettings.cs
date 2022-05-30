namespace ThirdParty.CSSuppliers.Restel
{

    public interface IRestelSettings
    {
        string Codusu(IThirdPartyAttributeSearch tpAttributeSearch);

        string Codigousu(IThirdPartyAttributeSearch tpAttributeSearch);

        string Clausu(IThirdPartyAttributeSearch tpAttributeSearch);

        string Secacc(IThirdPartyAttributeSearch tpAttributeSearch);

        string URL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        bool GuestNamesAvailable(IThirdPartyAttributeSearch tpAttributeSearch);

        bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch);

        bool CancelFirstPreBook(IThirdPartyAttributeSearch tpAttributeSearch);

        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);

        string Afil(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
