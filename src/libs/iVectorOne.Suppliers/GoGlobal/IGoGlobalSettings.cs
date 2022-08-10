namespace iVectorOne.Suppliers.GoGlobal 
{
    public interface IGoGlobalSettings
    {
        string Agency(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
