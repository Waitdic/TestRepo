namespace iVectorOne.Suppliers.TBOHolidays
{
    public interface ITBOHolidaysSettings
    {
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
        int RoomSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        string RequestedMealBases(IThirdPartyAttributeSearch tpAttributeSearch);


        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        string PaymentModeType(IThirdPartyAttributeSearch tpAttributeSearch);
        string ClientCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string ResultCount(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}