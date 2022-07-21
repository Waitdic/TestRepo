namespace iVectorOne.Suppliers.Acerooms
{
    using iVectorOne;

    public interface IAceroomsSettings
    {
        string APIKey(IThirdPartyAttributeSearch tpAttributeSearch);
        string Secret(IThirdPartyAttributeSearch tpAttributeSearch);
        string Signature(IThirdPartyAttributeSearch tpAttributeSearch);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}