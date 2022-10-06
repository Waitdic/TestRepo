namespace iVectorOne.Suppliers.Italcamel
{
    public interface IItalcamelSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string Username(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}

