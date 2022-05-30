namespace ThirdParty.CSSuppliers.DOTW
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedDOTWSettings : SettingsBase, IDOTWSettings
    {
        protected override string Source => ThirdParties.DOTW;

        public string CompanyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CompanyCode", tpAttributeSearch);           
        }

        public string CustomerCountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("CustomerCountryCode", tpAttributeSearch);
        }

        public string CustomerNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
             return Get_Value("CustomerNationality", tpAttributeSearch);
        }

        public int DefaultCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch)
        {

            return Get_Value("DefaultCurrencyID", tpAttributeSearch).ToSafeInt();
        }

        public bool ExcludeDOTWThirdParties(IThirdPartyAttributeSearch tpAttributeSearch)
        {

            return Get_Value("ExcludeDOTWThirdParties", tpAttributeSearch).ToSafeBoolean();
            
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {

            return Get_Value("Password", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {

            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string RequestCurrencyID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestCurrencyID", tpAttributeSearch);
            
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool SendTradeReference(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SendTradeReference", tpAttributeSearch).ToSafeBoolean();
        }

        public string ServerURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
           return Get_Value("ServerURL", tpAttributeSearch);
        }

        public string ThreadedSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ThreadedSearch", tpAttributeSearch);
        }

        public bool UseMinimumSellingPrice(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseMinimumSellingPrice", tpAttributeSearch).ToSafeBoolean();
        }

        public string Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {

            return Get_Value("Username", tpAttributeSearch);
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch);
        }
    }
}
