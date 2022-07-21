namespace iVectorOne.Suppliers.MTS
{
    using iVectorOne;

    public interface IMTSSettings
    {
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        int Type(IThirdPartyAttributeSearch tpAttributeSearch);
        string Instance(IThirdPartyAttributeSearch tpAttributeSearch);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int Unique_ID_Type(IThirdPartyAttributeSearch tpAttributeSearch);
        string OverRideID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        int AuthenticationType(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string ID_Context(IThirdPartyAttributeSearch tpAttributeSearch);
        string OverrideCountries(IThirdPartyAttributeSearch tpAttributeSearch);
        string AuthenticationID(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}