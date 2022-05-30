namespace ThirdParty.CSSuppliers
{

    public interface ITravelgateSettings
    {
        string get_Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SearchSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_PrebookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_BookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        bool get_RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_ReferenceDelimiter(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        string get_DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        string get_CardHolderName(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_ProviderUsername(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_ProviderPassword(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_ProviderCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_UrlReservation(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_UrlGeneric(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_UrlValuation(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_UrlAvail(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Parameters(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Markets(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_MaximumHotelSearchNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_MaximumCitySearchNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_MaximumRoomNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_MinimumStay(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_UseZoneSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_SearchRequestTimeout(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_SendGUIDReference(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}