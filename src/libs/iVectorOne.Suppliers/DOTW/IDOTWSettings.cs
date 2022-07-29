namespace iVectorOne.Suppliers.DOTW
{
    public interface IDOTWSettings
    {
        string CompanyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        int DefaultCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeDOTWThirdParties(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string RequestCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string ThreadedSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseMinimumSellingPrice(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        int Version(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}