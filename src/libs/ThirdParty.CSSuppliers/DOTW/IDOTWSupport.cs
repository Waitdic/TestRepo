namespace ThirdParty.CSSuppliers.DOTW
{
    using System.Threading.Tasks;

    public interface IDOTWSupport
    {
        string MD5Password(string password);

        int GetTitleID(string sTitle);

        int GetCurrencyID(IThirdPartyAttributeSearch searchDetails);

        Task<int> GetCurrencyCodeAsync(int currencyId, IThirdPartyAttributeSearch searchDetails);

        string CleanName(string name);
    }
}