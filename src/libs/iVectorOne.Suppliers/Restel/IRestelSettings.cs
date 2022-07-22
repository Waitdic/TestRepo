namespace iVectorOne.CSSuppliers.Restel
{

    public interface IRestelSettings
    {
        string Codusu(IThirdPartyAttributeSearch tpAttributeSearch);

        string Codigousu(IThirdPartyAttributeSearch tpAttributeSearch);

        string Clausu(IThirdPartyAttributeSearch tpAttributeSearch);

        string Secacc(IThirdPartyAttributeSearch tpAttributeSearch);

        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        bool GuestNamesAvailable(IThirdPartyAttributeSearch tpAttributeSearch);

        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);

        bool CancelFirstPreBook(IThirdPartyAttributeSearch tpAttributeSearch);

        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);

        string Afil(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
