namespace ThirdParty.CSSuppliers.Bonotel
{
    public interface IBonotelSettings
    {
        string get_URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        int get_BookTimeout(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}