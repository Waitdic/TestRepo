namespace ThirdParty.CSSuppliers.HotelsProV2
{
    public interface IHotelsProV2Settings
    {
        string BookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelAvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string ProvisionURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string UserName(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string Nationality(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseMultiHotelCodesSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaxHotelCodesPerRequest(IThirdPartyAttributeSearch tpAttributeSearch);

    }
}
