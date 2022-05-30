namespace ThirdParty.CSSuppliers
{
    using ThirdParty;

    public interface IYalagoSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);

        string API_Key(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch);

        bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch);

        string Language(IThirdPartyAttributeSearch tpAttributeSearch);

        string CountryCode(IThirdPartyAttributeSearch tpAttributeSearch);

        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string PreCancelURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch);

        string PreBookURL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
