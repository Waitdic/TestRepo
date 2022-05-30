namespace ThirdParty.CSSuppliers.Stuba
{

    public interface IStubaSettings
    {
        string get_URL(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_MaxHotelsPerRequest(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Organisation(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Version(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Nationality(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_ExcludeNonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_ExcludeUnknownCancellationPolicys(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
    }
}