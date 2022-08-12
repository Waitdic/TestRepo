namespace iVectorOne.Suppliers.OceanBeds
{
    using iVectorOne;

    public interface IOceanBedsSettings
    {
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch thirdPartyAttributeSearch);
        string ContactPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        string ContactEmail(IThirdPartyAttributeSearch tpAttributeSearch);
        string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationPolicyURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
