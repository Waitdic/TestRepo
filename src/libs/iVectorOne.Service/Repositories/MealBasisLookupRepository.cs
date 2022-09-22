namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using iVectorOne.Constants;
    using iVectorOne.Models;

    // todo - merge this file with the tp support wrapper
    /// <summary>
    /// A repository that looks up Meal Basis
    /// </summary>
    public class MealBasisLookupRepository : IMealBasisLookupRepository
    {
        /// <summary>The cache key</summary>
        private const string CacheKey = "APIMealBasisRepo";

        private const int _timeout = 2;

        private readonly IMemoryCache _cache;
        private readonly ISql _sql;

        public MealBasisLookupRepository(IMemoryCache cache, ISql sql)
        {
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <inheritdoc />
        public async Task<string> GetMealBasisfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis, int accountId)
        {
            if (IsSingleTenant(source))
            {
                return (await AccountMealBasisLookupAsync(accountId, thirdPartyMealBasis))?.MealBasis ?? string.Empty;
            }
            else
            {
                return (await GetMealBasisFromCodeAsync(source, thirdPartyMealBasis))?.MealBasisName ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public async Task<int> GetMealBasisIDfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis, int accountId)
        {
            if (IsSingleTenant(source))
            {
                return (await AccountMealBasisLookupAsync(accountId, thirdPartyMealBasis))?.AccountMealBasisID ?? 0;
            }
            else
            {
                return (await GetMealBasisFromCodeAsync(source, thirdPartyMealBasis))?.MealBasisID ?? 0;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetMealBasisCodefromTPMealbasisIDAsync(string source, int MealBasisID, int accountId)
        {
            if (IsSingleTenant(source))
            {
                var mealBasis = (await AccountMealBasisAsync())
                    .FirstOrDefault(x => x.Value.AccountID == accountId &&
                        x.Value.AccountMealBasisID == MealBasisID).Value;

                return mealBasis?.MealBasisCode ?? string.Empty;
            }
            else
            {
                var mealBasis = (await GetMealBasisAsync())
                    .FirstOrDefault(c => c.Value.Source == source &&
                        c.Value.MealBasisID == MealBasisID).Value;

                return mealBasis?.MealBasisCode ?? string.Empty;
            }
        }

        private async Task<AccountMealBasis> AccountMealBasisLookupAsync(int accountId, string mealBasisCode)
        {
            var cache = await AccountMealBasisAsync();
            cache.TryGetValue((accountId, mealBasisCode), out var accountMealBasis);

            return accountMealBasis;
        }

        private async Task<Dictionary<(int, string), AccountMealBasis>> AccountMealBasisAsync()
        {
            string cacheKey = "AccountMealBasisLookup";

            async Task<Dictionary<(int, string), AccountMealBasis>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select AccountID, MealBasisCode, MealBasis, AccountMealBasisID from AccountMealBasis",
                    async r => (await r.ReadAllAsync<AccountMealBasis>())
                        .ToDictionary(x => (x.AccountID, x.MealBasisCode), x => x));
            }

            var cache = await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);

            return cache;
        }

        private class AccountMealBasis
        {
            public int AccountID { get; set; }
            public string MealBasisCode { get; set; } = string.Empty;
            public string MealBasis { get; set; } = string.Empty;
            public int AccountMealBasisID { get; set; }
        }

        private bool IsSingleTenant(string source)
            => source.InList(ThirdParties.OWNSTOCK, ThirdParties.CHANNELMANAGER);

        private async Task<MealBasis> GetMealBasisFromCodeAsync(string source, string thirdPartyMealBasis)
        {
            (await GetMealBasisAsync()).TryGetValue((source, thirdPartyMealBasis), out var mealBasis);

            return mealBasis;
        }

        /// <summary>
        /// Gets a list of currencies
        /// </summary>
        /// <returns>
        ///   <br />
        /// </returns>
        private async Task<Dictionary<(string, string), MealBasis>> GetMealBasisAsync()
        {
            async Task<Dictionary<(string, string), MealBasis>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "Get_TPMealBasisCache",
                    async r => (await r.ReadAllAsync<MealBasisResult>())
                        .ToDictionary(x => (x.Source, x.MealBasisCode), x => x.ToMealBasis()));
            }

            return await _cache.GetOrCreateAsync(CacheKey, cacheBuilder, _timeout);
        }

        private class MealBasisResult
        {
            public int MealBasisID { get; set; }

            public string Source { get; set; } = string.Empty;

            public string MealBasisCode { get; set; } = string.Empty;

            public string MealBasis { get; set; } = string.Empty;

            internal MealBasis ToMealBasis()
            {
                return new MealBasis()
                {
                    MealBasisID = MealBasisID,
                    Source = Source,
                    MealBasisCode = MealBasisCode,
                    MealBasisName = MealBasis,
                };
            }
        }
    }
}