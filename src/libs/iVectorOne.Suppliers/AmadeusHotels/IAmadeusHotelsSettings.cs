namespace iVectorOne.Suppliers.AmadeusHotels
{
    public interface IAmadeusHotelsSettings
    {
        bool PostBookPNRAddMultiElement();
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string UserID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool SplitMultiRoom(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        string OfficeID(IThirdPartyAttributeSearch tpAttributeSearch);
        string CreditCardNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchCacheLevel(IThirdPartyAttributeSearch tpAttributeSearch);
        string BillingMethod(IThirdPartyAttributeSearch tpAttributeSearch);
        bool IncludeRoomTypeCodeInSearch(IThirdPartyAttributeSearch tpAttributeSearch);
        string ReceivedFrom(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludePackageRates(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentDutyCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchMode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch);
        bool RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch);
        string Telephone(IThirdPartyAttributeSearch tpAttributeSearch);
        string Email(IThirdPartyAttributeSearch tpAttributeSearch);
        string CID(IThirdPartyAttributeSearch tpAttributeSearch);
        string EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaxPages(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}