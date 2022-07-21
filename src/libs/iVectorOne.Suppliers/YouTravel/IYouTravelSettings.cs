namespace iVectorOne.CSSuppliers.YouTravel
{
    public interface IYouTravelSettings
    {
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationFeeURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationPolicyURL(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}