namespace ThirdParty.CSSuppliers.Stuba
{

    public interface IStubaSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
        string Organisation(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeUnknownCancellationPolicys(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
    }
}