namespace iVectorOne.Suppliers
{
    using iVectorOne;

    public interface IYalagoSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string APIKey(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch);

        bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch);

        string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch);

        string SourceMarket(IThirdPartyAttributeSearch tpAttributeSearch);

        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string PreCancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
