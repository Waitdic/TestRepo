namespace iVectorOne.CSSuppliers.FastPayHotels
{
    public interface IFastPayHotelsSettings
    {
        string ClientId(IThirdPartyAttributeSearch tpAttributeSearch);
        string ClientSecret(IThirdPartyAttributeSearch tpAttributeSearch);
        string XMLUser(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string UserRateType(IThirdPartyAttributeSearch tpAttributeSearch);
        string DelimChar(IThirdPartyAttributeSearch tpAttributeSearch); // deliminates the availability and reservation tokens
        string AccessToken(IThirdPartyAttributeSearch tpAttributeSearch); 
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch); 
        string CountryOfResidence(IThirdPartyAttributeSearch tpAttributeSearch); 
    }
}
