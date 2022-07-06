namespace ThirdParty.CSSuppliers.Hotelston
{
    public interface IHotelstonSettings
    {
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string Email(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string NetRates(IThirdPartyAttributeSearch tpAttributeSearch);
        string EndPointUrl(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string SoapRequestUrl(IThirdPartyAttributeSearch tpAttributeSearch);
        string SoapTypesUrl(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AlwaysUseDefaultEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultFirstName(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultLastName(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}