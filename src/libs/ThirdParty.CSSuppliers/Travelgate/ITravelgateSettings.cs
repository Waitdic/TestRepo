namespace ThirdParty.CSSuppliers.Travelgate
{
    public interface ITravelgateSettings
    {
        string get_Username(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_URL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_SearchSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_PrebookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_BookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_CancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool get_RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_ReferenceDelimiter(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_CardHolderName(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_ProviderUsername(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_ProviderPassword(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_ProviderCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_UrlReservation(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_UrlGeneric(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_UrlValuation(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_UrlAvail(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_Parameters(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_Markets(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_MaximumHotelSearchNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_MaximumCitySearchNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_MaximumRoomNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_MinimumStay(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool get_AllowHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool get_UseZoneSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int get_SearchRequestTimeout(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string get_RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool get_SendGUIDReference(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}