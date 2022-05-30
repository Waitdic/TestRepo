namespace ThirdParty.CSSuppliers.YouTravel
{
    public interface IYouTravelSettings
    {
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        string get_Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CancellationFeeURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_LangID(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CancellationPolicyURL(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}