namespace iVectorOne.Suppliers.HotelsProV2
{
    public interface IHotelsProV2Settings
    {
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch); 
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        bool EnableHotelSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
