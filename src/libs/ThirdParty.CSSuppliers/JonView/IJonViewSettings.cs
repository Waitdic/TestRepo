namespace ThirdParty.CSSuppliers.JonView
{

    public interface IJonViewSettings
    {
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        string get_URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_UserID(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_ClientLoc(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}