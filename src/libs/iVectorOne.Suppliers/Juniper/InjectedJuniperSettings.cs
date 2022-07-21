namespace iVectorOne.CSSuppliers.Juniper
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Support;

    public class InjectedJuniperSettings : SettingsBase, IJuniperSettings
    {
        protected override string Source => string.Empty;

        public string AgentDutyCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentDutyCode", tpAttributeSearch, source);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("BaseURL", tpAttributeSearch, source);
        }

        public string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch, source);
        }

        public bool ExcludeNonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ExcludeNonRefundableRates", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string HotelAvailURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelAvailURL", tpAttributeSearch, source);
        }

        public string HotelAvailURLSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelAvailURLSOAPAction", tpAttributeSearch, source);
        }

        public string HotelBookingRuleSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBookingRuleSOAPAction", tpAttributeSearch, source);
        }

        public string HotelBookingRuleURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBookingRuleURL", tpAttributeSearch, source);
        }

        public string HotelBookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBookSOAPAction", tpAttributeSearch, source);
        }

        public string HotelBookURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelBookURL", tpAttributeSearch, source);
        }

        public string HotelCancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelCancelSOAPAction", tpAttributeSearch, source);
        }

        public string HotelCancelURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelCancelURL", tpAttributeSearch, source);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("LanguageCode", tpAttributeSearch, source);
        }

        public int MaxHotelsPerSearchRequest(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaxHotelsPerSearchRequest", tpAttributeSearch, source).ToSafeInt();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string PaxCountry(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PaxCountry", tpAttributeSearch, source);
        }

        public bool ShowCatalogueData(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ShowCatalogueData", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SplitMultiroom", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseGZip", tpAttributeSearch, source).ToSafeBoolean();
        }
    }
}