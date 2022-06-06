namespace ThirdParty.CSSuppliers.Travelgate
{
    public interface ITravelgateArabianASettings : ITravelgateSettings
    {
    }

    public interface ITravelgateBookohotelSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDarinaSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDerbysoftSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDerbysoftBestWesternSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDerbysoftIHGSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDerbysoftNAVHSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDerbysoftUORSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDingusSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDingusBlueSeaSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDingusSpringHotelsSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDingusTHBSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateDOTWv3Settings : ITravelgateSettings
    {
    }

    public interface ITravelgateEETGlobalSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateEuroPlayasSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateGekkoSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateHotelTraderSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateIxpiraSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateMethabookSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateOswaldArrigoSettings : ITravelgateSettings
    {
    }

    public interface ITravelgatePerlatoursSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateTBOSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateTravellandaSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateTraveltinoSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateViajesOlympiaSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateWHLSettings : ITravelgateSettings
    {
    }

    public interface ITravelgateYalagoSettings : ITravelgateSettings
    {
    }

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