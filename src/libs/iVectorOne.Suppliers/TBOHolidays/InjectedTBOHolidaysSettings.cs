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
            throw new System.NotImplementedException();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public int RoomSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public string PaymentModeType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PaymentModeType", tpAttributeSearch);
        }

        public string ClientCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientCode", tpAttributeSearch);
        }

        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LeadGuestNationality", tpAttributeSearch);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string ResultCount(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ResultCount", tpAttributeSearch);
        }
    }
}
