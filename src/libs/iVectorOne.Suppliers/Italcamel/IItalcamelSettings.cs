namespace iVectorOne.Suppliers.Italcamel
{
    public interface IItalcamelSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Login(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaximumHotelSearchNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaximumNumberOfNights(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaximumRoomNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch);
        bool PackageRates(IThirdPartyAttributeSearch tpAttributeSearch);
        decimal DeltaPrice(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}

