﻿namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using iVectorOne.Models;

    // todo - merge this file with the tp support wrapper
    /// <summary>
    /// A repository that looks up currencies
    /// </summary>
    public class CurrencyLookupRepository : ICurrencyLookupRepository
    {
        /// <summary>The cache key</summary>
        private const string _cacheKey = "APICurencyRepo";
        private const int _timeout = 2;

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

            return await _cache.GetOrCreateAsync(_cacheKey, cacheBuilder, _timeout);
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
        public async Task<int> AccountCurrencyLookupAsync(int accountId, string currencyCode)
        {
            string cacheKey = "AccountCurrencyLookup";

            async Task<Dictionary<(int, string), int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select AccountID, CurrencyCode, ISOCurrencyID from AccountCurrency",
                    async r => (await r.ReadAllAsync<AccountCurrency>())
                        .ToDictionary(x => (x.AccountID, x.CurrencyCode), x => x.ISOCurrencyID));
            }

            var cache = await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
            cache.TryGetValue((accountId, currencyCode), out int isoCurrencyID);

            return isoCurrencyID;
        }

        public class AccountCurrency
        {
            public int AccountID { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
            public int ISOCurrencyID { get; set; }
        }
    }
}