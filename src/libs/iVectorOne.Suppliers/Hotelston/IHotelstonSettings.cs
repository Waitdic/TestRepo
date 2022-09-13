namespace iVectorOne.Suppliers.Hotelston
{
    public interface IHotelstonSettings
    {
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string ContactEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string NetRates(IThirdPartyAttributeSearch tpAttributeSearch);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPRequestURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string SOAPTypesURL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AlwaysUseDefaultEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultFirstName(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultLastName(IThirdPartyAttributeSearch tpAttributeSearch);
        string ContactPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}