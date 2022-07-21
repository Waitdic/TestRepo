namespace ThirdParty.CSSuppliers.TBOHolidays
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedTBOHolidaysSettings : SettingsBase, ITBOHolidaysSettings
    {
        protected override string Source => ThirdParties.TBOHOLIDAYS;

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string UserName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserName", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string PaymentModeType(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PaymentModeType", tpAttributeSearch);
        }

        public string ClientReferenceNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientReferenceNumber", tpAttributeSearch);
        }

        public string DefaultGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultGuestNationality", tpAttributeSearch);
        }

        public string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch);
        }

        public string ResultCount(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ResultCount", tpAttributeSearch);
        }
    }
}
