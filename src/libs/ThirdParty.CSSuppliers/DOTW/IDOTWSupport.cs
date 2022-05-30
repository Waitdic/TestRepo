namespace ThirdParty.CSSuppliers.DOTW
{
    using ThirdParty.Lookups;

    public interface IDOTWSupport
    {
        int GetCachedCurrencyID(IThirdPartyAttributeSearch SearchDetails, ITPSupport Support, string CurrencyCode, IDOTWSettings Settings);
    }
}
