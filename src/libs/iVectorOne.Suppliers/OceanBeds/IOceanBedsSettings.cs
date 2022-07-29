namespace iVectorOne.Suppliers.OceanBeds
{
    using iVectorOne;

    public interface IOceanBedsSettings
    {
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string SearchEndPoint(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGzip(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string Telephone(IThirdPartyAttributeSearch tpAttributeSearch);
        string DefaultEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookEndPoint(IThirdPartyAttributeSearch tpAttributeSearch);
        string GetCancellationEndPoint(IThirdPartyAttributeSearch tpAttributeSearch);
        string ConfirmCancelEndPoint(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
