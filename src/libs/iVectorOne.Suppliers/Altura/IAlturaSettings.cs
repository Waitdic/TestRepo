namespace iVectorOne.CSSuppliers.Altura
{
    using iVectorOne;
    public interface IAlturaSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgencyId(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string ExternalId(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        bool EnableMultiRoomSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}