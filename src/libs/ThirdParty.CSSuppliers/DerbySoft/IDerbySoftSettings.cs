namespace ThirdParty.CSSuppliers.DerbySoft
{
    public interface IDerbySoftClubMedSettings : IDerbySoftSettings
    {
    }

    public interface IDerbySoftMarriottSettings : IDerbySoftSettings
    {
    }

    public interface IDerbySoftSynxisSettings : IDerbySoftSettings
    {
    }

    public interface IDerbySoftSettings
    {
        string SupplierID(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string User(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string Password(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string PreBookURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string BookURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string CancelURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        int HotelSearchLimit(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        bool ExcludeNonRefundable(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        bool UseShoppingEngineForSearch(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string ShoppingEngineURL(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string ShoppingEnginePassword(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        int ShoppingEngineHotelsBatchSize(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
    }
}