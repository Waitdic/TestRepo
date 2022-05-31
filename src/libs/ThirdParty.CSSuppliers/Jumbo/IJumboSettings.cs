namespace ThirdParty.CSSuppliers.Jumbo
{
    public interface IJumboSettings
    {
        string get_AgencyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_BrandCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_POS(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CommonsURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_HotelBookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_BasketHandlerURL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Language(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        string get_NationalityBasedCredentials(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}