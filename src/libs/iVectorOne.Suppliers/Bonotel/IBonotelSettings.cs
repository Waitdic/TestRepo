namespace iVectorOne.CSSuppliers.Bonotel
{
    public interface IBonotelSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        int BookTimeout(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}