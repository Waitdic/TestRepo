namespace iVectorOne.Suppliers.DerbySoft
{
    public interface IDerbySoftSettings
    {
        string SupplierID(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string User(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string PrebookURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string BookingURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string CancellationURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string SearchURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        int HotelBatchLimit(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool ExcludeNRF(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool EnableUtilitySearch(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string SecondarySearchURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string SecondaryPassword(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool AllowCancellations(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        int SecondaryHotelBatchLimit(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
    }
}