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

    // todo - merge this file with the tp support wrapper
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

        /// <inheritdoc />
        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            async Task<List<Currency>> cacheBuilder()
            {
                return (await _sql.ReadAllAsync<Currency>("Currency_List")).ToList();
            }

            return await _cache.GetOrCreateAsync(_cacheKey, cacheBuilder, 60);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<int> SubscriptionCurrencyLookupAsync(int subscriptionId, string currencyCode)
        {
            string cacheKey = "SubscriptionCurrencyLookup";

            async Task<Dictionary<(int, string), int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select SubscriptionID, CurrencyCode, ISOCurrencyID from SubscriptionCurrency",
                    async r => (await r.ReadAllAsync<SubscriptionCurrency>())
                        .ToDictionary(x => (x.SubscriptionID, x.CurrencyCode), x => x.ISOCurrencyID));
            }

            var cache = await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
            cache.TryGetValue((subscriptionId, currencyCode), out int isoCurrencyID);

            return isoCurrencyID;
        }

        public class SubscriptionCurrency
        {
            public int SubscriptionID { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
            public int ISOCurrencyID { get; set; }
        }
    }
}