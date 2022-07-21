namespace ThirdParty.CSSuppliers.JonView
{
    public interface IJonViewSettings
    {
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string ClientLoc(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}