namespace iVectorOne.Suppliers.Travelgate
{
    public interface ITravelgateSettings
    {
        string User(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string ReferenceDelimiter(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string CardHolderName(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SecondaryUsername(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SecondaryPassword(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string ProviderCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SecondaryBookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SecondaryGenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SecondaryPrebookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SecondarySearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Parameters(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int CityBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int RoomSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int MinimumStay(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool EnableHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseZoneSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int SearchRequestTimeout(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool SendGUIDReference(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}