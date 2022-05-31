namespace ThirdParty.CSSuppliers.Juniper
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers;
    using ThirdParty.Support;

    public class InjectedJuniperECTravelSettings : InjectedJuniperBaseSettings, IJuniperECTravelSettings
    {
        protected override string Source => ThirdParties.JUNIPERECTRAVEL;
    }

    public class InjectedJuniperElevateSettings : InjectedJuniperBaseSettings, IJuniperElevateSettings
    {
        protected override string Source => ThirdParties.JUNIPERELEVATE;
    }

    public class InjectedJuniperFastPayHotelsSettings : InjectedJuniperBaseSettings, IJuniperFastPayHotelsSettings
    {
        protected override string Source => ThirdParties.JUNIPERFASTPAYHOTELS;
    }

    public abstract class InjectedJuniperBaseSettings : SettingsBase, IJuniperBaseSettings
    {
        public string AgentDutyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentDutyCode", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BaseURL", tpAttributeSearch);
        }

        public string CurrencyCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CurrencyCode", tpAttributeSearch);
        }

        public bool ExcludeNonRefundableRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNonRefundableRates", tpAttributeSearch).ToSafeBoolean();
        }

        public string HotelAvailURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelAvailURL", tpAttributeSearch);
        }

        public string HotelAvailURLSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelAvailURLSOAPAction", tpAttributeSearch);
        }

        public string HotelBookingRuleSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBookingRuleSOAPAction", tpAttributeSearch);
        }

        public string HotelBookingRuleURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBookingRuleURL", tpAttributeSearch);
        }

        public string HotelBookSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBookSOAPAction", tpAttributeSearch);
        }

        public string HotelBookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBookURL", tpAttributeSearch);
        }

        public string HotelCancelSOAPAction(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelCancelSOAPAction", tpAttributeSearch);
        }

        public string HotelCancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelCancelURL", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public int MaxHotelsPerSearchRequest(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaxHotelsPerSearchRequest", tpAttributeSearch).ToSafeInt();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string PaxCountry(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PaxCountry", tpAttributeSearch);
        }

        public bool ShowCatalogueData(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ShowCatalogueData", tpAttributeSearch).ToSafeBoolean();
        }

        public bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SplitMultiroom", tpAttributeSearch).ToSafeBoolean();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
