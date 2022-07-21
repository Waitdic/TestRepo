namespace iVectorOne.Suppliers.TBOHolidays
{
    public interface ITBOHolidaysSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string UserName(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string PaymentModeType(IThirdPartyAttributeSearch tpAttributeSearch);
        string ClientReferenceNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch);
        string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string ResultCount(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}