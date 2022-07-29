namespace iVectorOne.Suppliers.Travelgate
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Support;

    public class InjectedTravelgateSettings : SettingsBase, ITravelgateSettings
    {
        protected override string Source => string.Empty;

        public string User(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("User", tpAttributeSearch, source);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("GenericURL", tpAttributeSearch, source);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SearchURL", tpAttributeSearch, source);
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PrebookURL", tpAttributeSearch, source);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("BookingURL", tpAttributeSearch, source);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CancellationURL", tpAttributeSearch, source);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseGZIP", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public bool RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RequiresVCard", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string ReferenceDelimiter(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ReferenceDelimiter", tpAttributeSearch, source);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch, source);
        }

        public string CardHolderName(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CardHolderName", tpAttributeSearch, source);
        }

        public string EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("EncryptedCardDetails", tpAttributeSearch, source);
        }

        public string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SourceMarket", tpAttributeSearch, source);
        }

        public string SecondaryUsername(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondaryUsername", tpAttributeSearch, source);
        }

        public string SecondaryPassword(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondaryPassword", tpAttributeSearch, source);
        }

        public string ProviderCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ProviderCode", tpAttributeSearch, source);
        }

        public string SecondaryBookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UrlReservation", tpAttributeSearch, source);
        }

        public string SecondaryGenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondaryGenericURL", tpAttributeSearch, source);
        }

        public string SecondaryPrebookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondaryPrebookURL", tpAttributeSearch, source);
        }

        public string SecondarySearchURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SecondarySearchURL", tpAttributeSearch, source);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LanguageCode", tpAttributeSearch, source);
        }

        public string Parameters(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Parameters", tpAttributeSearch, source);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch, source);
        }

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public int CityBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CityBatchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public int RoomSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RoomSearchLimit", tpAttributeSearch, source).ToSafeInt();
        }

        public int MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaximumRoomGuestNumber", tpAttributeSearch, source).ToSafeInt();
        }

        public int MinimumStay(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MinimumStay", tpAttributeSearch, source).ToSafeInt();
        }

        public bool EnableHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("EnableHotelSearch", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool UseZoneSearch(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseZoneSearch", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int SearchRequestTimeout(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SearchRequestTimeout", tpAttributeSearch, source).ToSafeInt();
        }

        public string RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RatePlanCodes", tpAttributeSearch, source);
        }

        public bool SendGUIDReference(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SendGUIDReference", tpAttributeSearch, source).ToSafeBoolean();
        }
    }
}