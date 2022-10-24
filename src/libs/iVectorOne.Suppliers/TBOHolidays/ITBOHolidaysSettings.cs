namespace iVectorOne.Suppliers.TBOHolidays
{
    public interface ITBOHolidaysSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string PaymentModeType(IThirdPartyAttributeSearch tpAttributeSearch);
        string ClientCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string ResultCount(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}