namespace iVectorOne.Suppliers
{
    using iVectorOne;
    public interface IWelcomeBedsSettings
    {
        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        public string Version(IThirdPartyAttributeSearch tpAttributeSearch);
        public string AccountCode(IThirdPartyAttributeSearch tpAttributeSearch);
        public string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        public string System(IThirdPartyAttributeSearch tpAttributeSearch);
        public string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch);
        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        public string Agency(IThirdPartyAttributeSearch tpAttributeSearch);
        public int ResortSearchSap(IThirdPartyAttributeSearch tpAttributeSearch);
        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        public int ResortSearchCap(IThirdPartyAttributeSearch tpAttributeSearch);
        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        public string ConnectionString(IThirdPartyAttributeSearch tpAttributeSearch);
        public bool RequiresVCard(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
