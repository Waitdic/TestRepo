namespace ThirdParty.CSSuppliers.MTS
{
    using ThirdParty;

    public interface IMTSSettings
    {
        string ID(IThirdPartyAttributeSearch tpAttributeSearch);
        int Type(IThirdPartyAttributeSearch tpAttributeSearch);
        string Instance(IThirdPartyAttributeSearch tpAttributeSearch);
        string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int Unique_ID_Type(IThirdPartyAttributeSearch tpAttributeSearch);
        string OverRideID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory);
        int AuthenticationType(IThirdPartyAttributeSearch tpAttributeSearch);
        string MessagePassword(IThirdPartyAttributeSearch tpAttributeSearch);
        string ID_Context(IThirdPartyAttributeSearch tpAttributeSearch);
        string OverrideCountries(IThirdPartyAttributeSearch tpAttributeSearch);
        string AuthenticationID(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}