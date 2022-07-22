namespace iVectorOne.Lookups
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The support functions for third parties
    /// </summary>
    public interface ITPSupport
    {
        /// <summary>
        /// The country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="isoCode">The iso country code.</param>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <returns>
        /// The third party country code
        /// </returns>
        Task<string> TPCountryCodeLookupAsync(string source, string isoCode, int subscriptionId);

        /// <summary>
        /// The country lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="isoCode">The iso country code.</param>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <returns>
        /// The third party country name
        /// </returns>
        Task<string> TPCountryLookupAsync(string source, string isoCode, int subscriptionId);

        /// <summary>
        /// The credit card lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="creditCardTypeId">The credit card type identifier.</param>
        /// <returns>credit card</returns>
        string TPCreditCardLookup(string source, int creditCardTypeId);

        /// <summary>
        /// The currency lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyCode">The third party currency code.</param>
        /// <returns>The ISO currency code.</returns>
        Task<string> TPCurrencyLookupAsync(string source, string currencyCode);

        /// <summary>
        /// The currency code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyCode">The ISO currency code.</param>
        /// <returns>The third party currency code.</returns>
        Task<string> TPCurrencyCodeLookupAsync(string source, string isoCode);

        /// <summary>
        /// The nationality lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nationality">The nationality.</param>
        /// <returns>The Nationality</returns>
        Task<string> TPNationalityLookupAsync(string source, string nationality);

        /// <summary>
        /// The meal bases.
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <returns>meal bases</returns>
        Task<Dictionary<string, int>> TPMealBasesAsync(string source);

        /// <summary>ISO currency id lookup.</summary>
        /// <param name="currencyCode">The ISO currency code.</param>
        /// <returns>The currency id of the corresponding currencyCode</returns>
        Task<int> ISOCurrencyIDLookupAsync(string currencyCode);

        /// <summary>ISO currency code lookup.</summary>
        /// <param name="currencyId">The ISO currency id.</param>
        /// <returns>The currency code of corresponding currency id</returns>
        Task<string> ISOCurrencyCodeLookupAsync(int currencyId);
    }
}