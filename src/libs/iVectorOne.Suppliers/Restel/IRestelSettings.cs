namespace iVectorOne.Suppliers.Restel
{

    public interface IRestelSettings
    {
        string UserAgent(IThirdPartyAttributeSearch tpAttributeSearch);

        string User(IThirdPartyAttributeSearch tpAttributeSearch);

        string Password(IThirdPartyAttributeSearch tpAttributeSearch);

        string AccessToken(IThirdPartyAttributeSearch tpAttributeSearch);

        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);

        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);

        bool GuestNamesAvailable(IThirdPartyAttributeSearch tpAttributeSearch);
             
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);

    }
}
