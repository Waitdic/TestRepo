namespace ThirdParty.CSSuppliers.DOTW
{
    public interface IDOTWSupport
    {
        string MD5Password(string password);

        int GetTitleID(string sTitle);

        int GetCurrencyID(IThirdPartyAttributeSearch searchDetails);

        int GetCurrencyCode(int currencyId, IThirdPartyAttributeSearch searchDetails);

        string CleanName(string name);
    }
}