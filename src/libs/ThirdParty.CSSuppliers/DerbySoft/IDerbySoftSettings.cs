namespace ThirdParty.CSSuppliers.DerbySoft
{
    public interface IDerbySoftSettings
    {
        string SupplierID(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string User(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string PreBookURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string BookURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string CancelURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string SearchURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        int HotelSearchLimit(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool ExcludeNonRefundable(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool UseShoppingEngineForSearch(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string ShoppingEngineURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        string ShoppingEnginePassword(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        bool AllowCancellations(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        int ShoppingEngineHotelsBatchSize(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch thirdPartyAttributeSearch, string source);
    }
}