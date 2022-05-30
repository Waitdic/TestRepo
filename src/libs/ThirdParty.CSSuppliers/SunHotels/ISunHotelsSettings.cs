namespace ThirdParty.CSSuppliers.SunHotels
{
    public interface ISunHotelsSettings
    {
        string get_Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_EmailAddress(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Language(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SupplierReference(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_BookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_CancelURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandotory);
        string get_Nationality(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_PreBookURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string get_AccommodationTypes(IThirdPartyAttributeSearch tpAttributeSearch);
        bool get_RequestPackageRates(IThirdPartyAttributeSearch tpAttributeSearch);
        int get_HotelRequestLimit(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}