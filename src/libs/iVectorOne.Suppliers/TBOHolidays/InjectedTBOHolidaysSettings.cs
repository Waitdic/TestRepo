namespace iVectorOne.Suppliers.TBOHolidays
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedTBOHolidaysSettings : SettingsBase, ITBOHolidaysSettings
    {
        protected override string Source => ThirdParties.TBOHOLIDAYS;

        public string RequestedMealBases(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestedMealBases", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch).ToSafeBoolean();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelSearchLimit", tpAttributeSearch).ToSafeInt();
        }

        public int RoomSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RoomSearchLimit", tpAttributeSearch).ToSafeInt();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookURL", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }
    }
}
