using Intuitive.Helpers.Extensions;
using ThirdParty.Constants;
using ThirdParty.Search.Settings;
using ThirdParty.Support;

namespace ThirdParty.CSSuppliers
{

    public class InjectedTravelgateSettings : SettingsBase, ITravelgateSettings
    {

        private readonly ThirdPartyConfiguration configuration;

        protected override string Source => ThirdParties.TRAVELGATE;

        public InjectedTravelgateSettings(ThirdPartyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string get_Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }

        public string get_Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string get_URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string get_SearchSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchSOAPAction", tpAttributeSearch);
        }

        public string get_PrebookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookSOAPAction", tpAttributeSearch);
        }

        public string get_BookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookSOAPAction", tpAttributeSearch);
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_CancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelSOAPAction", tpAttributeSearch);
        }

        public bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool get_RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequiresVCard", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_ReferenceDelimiter(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("ReferenceDelimiter", tpAttributeSearch);
        }

        public string get_DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("DefaultNationality", tpAttributeSearch);
        }

        public string get_CardHolderName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CardHolderName", tpAttributeSearch);
        }

        public string get_EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EncryptedCardDetails", tpAttributeSearch);
        }

        public string get_Markets(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Markets", tpAttributeSearch);
        }

        public string get_ProviderUsername(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ProviderUsername", tpAttributeSearch);
        }

        public string get_ProviderPassword(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ProviderPassword", tpAttributeSearch);
        }

        public string get_ProviderCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ProviderCode", tpAttributeSearch);
        }

        public string get_UrlReservation(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UrlReservation", tpAttributeSearch);
        }

        public string get_UrlGeneric(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UrlGeneric", tpAttributeSearch);
        }

        public string get_UrlValuation(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UrlValuation", tpAttributeSearch);
        }

        public string get_UrlAvail(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UrlAvail", tpAttributeSearch);
        }

        public string get_LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public string get_Parameters(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Parameters", tpAttributeSearch);
        }

        public string get_CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch);
        }

        public int get_MaximumHotelSearchNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumHotelSearchNumber", tpAttributeSearch).ToSafeInt();
        }

        public int get_MaximumCitySearchNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumCitySearchNumber", tpAttributeSearch).ToSafeInt();
        }

        public int get_MaximumRoomNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumRoomNumber", tpAttributeSearch).ToSafeInt();
        }

        public int get_MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumRoomGuestNumber", tpAttributeSearch).ToSafeInt();
        }

        public int get_MinimumStay(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MinimumStay", tpAttributeSearch).ToSafeInt();
        }

        public bool get_AllowHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowHotelSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public bool get_UseZoneSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseZoneSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_SearchRequestTimeout(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchRequestTimeout", tpAttributeSearch).ToSafeInt();
        }

        public string get_RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RatePlanCodes", tpAttributeSearch);
        }

        public bool get_SendGUIDReference(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SendGUIDReference", tpAttributeSearch).ToSafeBoolean();
        }
    }
}