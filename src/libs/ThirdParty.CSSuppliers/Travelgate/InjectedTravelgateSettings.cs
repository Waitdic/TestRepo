namespace ThirdParty.CSSuppliers.Travelgate
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Support;

    public class InjectedTravelgateSettings : SettingsBase, ITravelgateSettings
    {
        protected override string Source => string.Empty;

        public string get_Username(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Username", tpAttributeSearch, source);
        }

        public string get_Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string get_URL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("URL", tpAttributeSearch, source);
        }

        public string get_SearchSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SearchSOAPAction", tpAttributeSearch, source);
        }

        public string get_PrebookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PrebookSOAPAction", tpAttributeSearch, source);
        }

        public string get_BookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("BookSOAPAction", tpAttributeSearch, source);
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string get_CancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CancelSOAPAction", tpAttributeSearch, source);
        }

        public bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseGZIP", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public bool get_RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RequiresVCard", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string get_ReferenceDelimiter(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ReferenceDelimiter", tpAttributeSearch, source);
        }

        public string get_DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("DefaultNationality", tpAttributeSearch, source);
        }

        public string get_CardHolderName(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CardHolderName", tpAttributeSearch, source);
        }

        public string get_EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("EncryptedCardDetails", tpAttributeSearch, source);
        }

        public string get_Markets(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Markets", tpAttributeSearch, source);
        }

        public string get_ProviderUsername(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ProviderUsername", tpAttributeSearch, source);
        }

        public string get_ProviderPassword(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ProviderPassword", tpAttributeSearch, source);
        }

        public string get_ProviderCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ProviderCode", tpAttributeSearch, source);
        }

        public string get_UrlReservation(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UrlReservation", tpAttributeSearch, source);
        }

        public string get_UrlGeneric(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UrlGeneric", tpAttributeSearch, source);
        }

        public string get_UrlValuation(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UrlValuation", tpAttributeSearch, source);
        }

        public string get_UrlAvail(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UrlAvail", tpAttributeSearch, source);
        }

        public string get_LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LanguageCode", tpAttributeSearch, source);
        }

        public string get_Parameters(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Parameters", tpAttributeSearch, source);
        }

        public string get_CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch, source);
        }

        public int get_MaximumHotelSearchNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaximumHotelSearchNumber", tpAttributeSearch, source).ToSafeInt();
        }

        public int get_MaximumCitySearchNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaximumCitySearchNumber", tpAttributeSearch, source).ToSafeInt();
        }

        public int get_MaximumRoomNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaximumRoomNumber", tpAttributeSearch, source).ToSafeInt();
        }

        public int get_MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaximumRoomGuestNumber", tpAttributeSearch, source).ToSafeInt();
        }

        public int get_MinimumStay(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MinimumStay", tpAttributeSearch, source).ToSafeInt();
        }

        public bool get_AllowHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowHotelSearch", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool get_UseZoneSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseZoneSearch", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int get_SearchRequestTimeout(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SearchRequestTimeout", tpAttributeSearch, source).ToSafeInt();
        }

        public string get_RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RatePlanCodes", tpAttributeSearch, source);
        }

        public bool get_SendGUIDReference(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SendGUIDReference", tpAttributeSearch, source).ToSafeBoolean();
        }
    }
}