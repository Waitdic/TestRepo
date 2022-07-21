namespace iVectorOne.CSSuppliers.AmadeusHotels
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedAmadeusHotelsSettings : SettingsBase, IAmadeusHotelsSettings
    {
        protected override string Source => ThirdParties.AMADEUSHOTELS;

        public bool PostBookPNRAddMultiElement()
        {
            return true;
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string UserID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserID", tpAttributeSearch);
        }

        public bool SplitMultiRoom(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SplitMultiRoom", tpAttributeSearch).ToSafeBoolean();
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string OfficeID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OfficeID", tpAttributeSearch);
        }

        public string CreditCardNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CreditCardNumber", tpAttributeSearch);
        }

        public string SearchCacheLevel(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchCacheLevel", tpAttributeSearch);
        }

        public string BillingMethod(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BillingMethod", tpAttributeSearch);
        }

        public bool IncludeRoomTypeCodeInSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("IncludeRoomTypeCodeInSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public string ReceivedFrom(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ReceivedFrom", tpAttributeSearch);
        }

        public bool ExcludePackageRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludePackageRates", tpAttributeSearch).ToSafeBoolean();
        }

        public string AgentDutyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentDutyCode", tpAttributeSearch);
        }

        public string SearchMode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchMode", tpAttributeSearch);
        }

        public bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public bool RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequiresVCard", tpAttributeSearch).ToSafeBoolean();
        }

        public string Telephone(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Telephone", tpAttributeSearch);
        }

        public string Email(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Email", tpAttributeSearch);
        }

        public string CID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CID", tpAttributeSearch);
        }

        public string EncryptedCardDetails(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("EncryptedCardDetails", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public int MaxPages(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaxPages", tpAttributeSearch).ToSafeInt();
        }
    }
}