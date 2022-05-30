namespace ThirdParty.CSSuppliers
{
    using ThirdParty;
    public interface IW2MSettings
    {
        string Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string SearchUrl(IThirdPartyAttributeSearch tpAttributeSearch);
        string PreBookUrl(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookUrl(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string LangID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch);
        int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch);
        bool SplitMultiroom(IThirdPartyAttributeSearch tpAttributeSearch);
        string SoapActionPrefix(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultNationality(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
