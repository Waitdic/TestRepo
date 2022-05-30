namespace ThirdParty.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using ThirdParty.Models;

    /// <summary>
    /// A repository that looks up currencies
    /// </summary>
    public class CurrencyLookupRepository : ICurrencyLookupRepository
    {
        /// <summary>The cache key</summary>
        private const string _cacheKey = "APICurencyRepo";

        private readonly IMemoryCache _cache;
        private readonly ISql _sql;

        public CurrencyLookupRepository(IMemoryCache cache, ISql sql)
        {
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <summary>
        /// Gets a list of currencies
        /// </summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            async Task<List<Currency>> cacheBuilder()
            {
                return (await _sql.ReadAllAsync<Currency>("Currency_List")).ToList();
            }

            return await _cache.GetOrCreateAsync(_cacheKey, cacheBuilder, 60);
        }

        /// <summary>
        /// Gets the currency code from third party currency code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The third party currency code.</param>
        /// <returns>a currency code as string, for the specified source  third party currency code</returns>
        public async Task<string> GetCurrencyCodefromTPCurrencyCodeAsync(string source, string thirdPartyCurrencyCode)
        {
            string currencyCode = string.Empty;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.ThirdPartyCurrencyCode == thirdPartyCurrencyCode));

            if (currency != null)
            {
                currencyCode = currency.CurrencyCode;
            }

            return currencyCode;
        }

        /// <summary>
        /// Gets the currency ID from third party currency code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The third party currency code.</param>
        /// <returns>a currency code as string, for the specified source  third party currency code</returns>
        public async Task<int> GetCurrencyIDFromSupplierCurrencyCodeAsync(string source, string thirdPartyCurrencyCode)
        {
            int currencyID = 0;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.ThirdPartyCurrencyCode == thirdPartyCurrencyCode));

            if (currency != null)
            {
                currencyID = currency.CurrencyID;
            }

            return currencyID;
        }

        /// <summary>Gets the currency code from third party currency code.</summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyID">The currency ID</param>
        /// <returns>
        ///  a currency code for the specified source and currencyID
        /// </returns>
        public async Task<string> GetCurrencyCodeFromCurrencyIDAsync(string source, int currencyID)
        {
            string currencyCode = string.Empty;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.CurrencyID == currencyID));

            if (currency != null)
            {
                currencyCode = currency.CurrencyCode;
            }

            return currencyCode;
        }

        /// <summary>Gets the currency code from third party currency code.</summary>
        /// <param name="source">The source.</param>
        /// <param name="isoCurrencyID">The currency ID</param>
        /// <returns>
        ///  a currency code for the specified source and currencyID
        /// </returns>
        public async Task<string> GetCurrencyCodeFromISOCurrencyIDAsync(string source, int isoCurrencyID)
        {
            string currencyCode = string.Empty;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.ISOCurrencyID == isoCurrencyID));

            if (currency != null)
            {
                currencyCode = currency.CurrencyCode;
            }

            return currencyCode;
        }

        public async Task<int> GetISOCurrencyIDFromCurrencyIDAndSupplierAsync(string source, int currencyID)
        {
            int isocurrencyID = 0;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.CurrencyID == currencyID));

            if (currency != null)
            {
                isocurrencyID = currency.ISOCurrencyID;
            }

            return isocurrencyID;
        }

        /// <summary>
        /// Gets the currency ID from third party currency code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyCurrencyCode">The third party currency code.</param>
        /// <returns>a currency code as string, for the specified source  third party currency code</returns>
        public async Task<int> GetISOCurrencyIDFromSupplierCurrencyCodeAsync(string source, string thirdPartyCurrencyCode)
        {
            int isoCurrencyID = 0;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.ThirdPartyCurrencyCode == thirdPartyCurrencyCode));

            if (currency != null)
            {
                isoCurrencyID = currency.ISOCurrencyID;
            }

            return isoCurrencyID;
        }

        public async Task<int> GetCurrencyIDFromISOCurrencyIDAndSupplierAsync(string source, int isoCurrencyID)
        {
            int currencyID = 0;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.Source == source) && (c.ISOCurrencyID == isoCurrencyID));

            if (currency != null)
            {
                currencyID = currency.CurrencyID;
            }

            return currencyID;
        }

        public async Task<decimal> GetExchangeRateFromISOCurrencyIDAsync(int isoCurrencyID)
        {
            decimal exchangeRate = 1;

            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => (c.ISOCurrencyID == isoCurrencyID));

            if (currency != null)
            {
                exchangeRate = currency.ExchangeRate;
            }

            return exchangeRate;
        }

        public async Task<int> GetISOCurrencyIDFromISOCurrencyCodeAsync(string currencyCode)
        {
            int currencyID = 0;
            var currencies = await GetCurrenciesAsync();

            var currency = currencies.FirstOrDefault(c => c.CurrencyCode == currencyCode);

            if (currency != null)
            {
                currencyID = currency.ISOCurrencyID;
            }
            return currencyID;
        }
    }
}