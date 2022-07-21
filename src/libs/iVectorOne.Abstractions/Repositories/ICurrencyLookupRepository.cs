namespace ThirdParty.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdParty.Models;

    // todo - remove duplicate code between here and tp support
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
        /// Gets the  currency identifier from the third party code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The currency identifier.</param>
        /// <returns> a currency code as string, for the specified source currency identifier</returns>
        Task<int> GetISOCurrencyIDFromSupplierCurrencyCodeAsync(string source, string thirdPartyCurrencyCode);

        /// <summary>
        /// Gets the currency exchange rate
        /// </summary>
        /// <param name="isoCurrencyID">The currency identifier.</param>
        /// <returns> an exchange rate for the currencyid into GBP</returns>
        Task<decimal> GetExchangeRateFromISOCurrencyIDAsync(int isoCurrencyID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        Task<int> SubscriptionCurrencyLookupAsync(int subscriptionId, string currencyCode);
    }
}