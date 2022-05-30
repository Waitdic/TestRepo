namespace ThirdParty.CSSuppliers.DOTW
{

    public interface IDOTWSettings
    {
        string CompanyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string CustomerNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        int DefaultCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeDOTWThirdParties(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string RequestCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        bool SendTradeReference(IThirdPartyAttributeSearch tpAttributeSearch);
        string ServerURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string ThreadedSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseMinimumSellingPrice(IThirdPartyAttributeSearch tpAttributeSearch);
        string Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch);
    }

}
