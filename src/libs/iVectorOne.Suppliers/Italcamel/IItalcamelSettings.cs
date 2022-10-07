namespace iVectorOne.Suppliers.Italcamel
{
    public interface IItalcamelSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Login(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}

