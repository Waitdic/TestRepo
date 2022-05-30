namespace ThirdParty.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdParty.Models;

    /// <summary>
    /// Currency lookup repository
    /// </summary>
    public interface ICurrencyLookupRepository
    {
        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>a list of currencies</returns>
        Task<List<Currency>> GetCurrenciesAsync();

        /// <summary>
        /// Gets the currency code from third party currency code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The third party currency code.</param>
        /// <returns> a currency code as string, for the specified source third party currency code</returns>
        Task<string> GetCurrencyCodefromTPCurrencyCodeAsync(string source, string thirdPartyCurrencyCode);

        /// <summary>
        /// Gets the currency code from currency identifier.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyID">The currency identifier.</param>
        /// <returns> a currency code as string, for the specified source currency identifier</returns>
        Task<string> GetCurrencyCodeFromCurrencyIDAsync(string source, int currencyID);

        /// <summary>
        /// Gets the currency code from currency identifier.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="isoCurrencyID">The currency identifier.</param>
        /// <returns> a currency code as string, for the specified source currency identifier</returns>
        Task<string> GetCurrencyCodeFromISOCurrencyIDAsync(string source, int isoCurrencyID);

        /// <summary>
        /// Gets the currency ID from third party currency code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The third party currency code.</param>
        /// <returns> a currency code as string, for the specified source third party currency code</returns>
        Task<int> GetCurrencyIDFromSupplierCurrencyCodeAsync(string source, string thirdPartyCurrencyCode);

        /// <summary>
        /// Gets the ISO currency identifier from the third party code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyID">The currency identifier.</param>
        /// <returns> a currency code as string, for the specified source currency identifier</returns>
        Task<int> GetISOCurrencyIDFromCurrencyIDAndSupplierAsync(string source, int currencyID);

        /// <summary>
        /// Gets the  currency identifier from the third party code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The currency identifier.</param>
        /// <returns> a currency code as string, for the specified source currency identifier</returns>
        Task<int> GetISOCurrencyIDFromSupplierCurrencyCodeAsync(string source, string thirdPartyCurrencyCode);

        /// <summary>
        /// Gets the currency code from currency identifier.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="isoCurrencyID">The currency identifier.</param>
        /// <returns> a currency code as string, for the specified source currency identifier</returns>
        Task<int> GetCurrencyIDFromISOCurrencyIDAndSupplierAsync(string source, int isoCurrencyID);

        /// <summary>
        /// Gets the currency exchange rate
        /// </summary>
        /// <param name="isoCurrencyID">The currency identifier.</param>
        /// <returns> an exchange rate for the currencyid into GBP</returns>
        Task<decimal> GetExchangeRateFromISOCurrencyIDAsync(int isoCurrencyID);

        /// <summary>
        /// Gets the ISOCurrencyID from the ISO currency code
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns>The id for the iso currency code</returns>
        Task<int> GetISOCurrencyIDFromISOCurrencyCodeAsync(string currencyCode);
    }
}