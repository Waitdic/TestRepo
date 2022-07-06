namespace ThirdParty.CSSuppliers.Jumbo
{
    public interface IJumboSettings
    {
        string AgencyID(IThirdPartyAttributeSearch tpAttributeSearch);
        string BrandCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string POS(IThirdPartyAttributeSearch tpAttributeSearch);
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        string NationalityBasedCredentials(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}