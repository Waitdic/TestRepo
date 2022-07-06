namespace ThirdParty.CSSuppliers.Miki
{
    public interface IMikiSettings
    {
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
        public string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch);
        public string BookingCountryCode (IThirdPartyAttributeSearch tpAttributeSearch);
        public string Language (IThirdPartyAttributeSearch tpAttributeSearch);
        public bool SendTradeReference (IThirdPartyAttributeSearch tpAttributeSearch);
        public string AgentCodeUSD (IThirdPartyAttributeSearch tpAttributeSearch);
        public string AgentCodeEUR (IThirdPartyAttributeSearch tpAttributeSearch);
        public string AgentCodeGBP (IThirdPartyAttributeSearch tpAttributeSearch);
        public string AccessCodesFilename (IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
