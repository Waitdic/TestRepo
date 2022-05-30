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
    /// A repository that looks up Meal Basis
    /// </summary>
    public class MealBasisLookupRepository : IMealBasisLookupRepository
    {
        /// <summary>The cache key</summary>
        private const string CacheKey = "APIMealBasisRepo";

        private readonly IMemoryCache _cache;
        private readonly ISql _sql;

        public MealBasisLookupRepository(IMemoryCache cache, ISql sql)
        {
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <summary>
        /// Gets the mealbasis code from third party currency code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party mealbasis code.</param>
        /// <returns>a mealbasis code as string, for the specified source  third party mealbasis code</returns>
        public async Task<string> GetMealBasisfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis)
        {
            var mealbasis = string.Empty;

            var mealBases = await GetMealBasisAsync();

            MealBasis mealBasis;

            mealBases.TryGetValue((source, thirdPartyMealBasis), out mealBasis);

            if (mealBasis != null)
            {
                mealbasis = mealBasis.MealBasisName;
            }

            return mealbasis;
        }

        /// <summary>
        /// Gets the currency mealbasisID from third party mealbasis code.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party mealbasis code.</param>
        /// <returns>a mealbasisID code as integer, for the specified source  third party mealbasis code</returns>
        public async Task<int> GetMealBasisIDfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis)
        {
            var mealbasis = 0;

            var mealBases = await GetMealBasisAsync();

            MealBasis mealBasis;

            mealBases.TryGetValue((source, thirdPartyMealBasis), out mealBasis);

            if (mealBasis != null)
            {
                mealbasis = mealBasis.MealBasisID;
            }

            return mealbasis;
        }

        /// <summary>
        /// Gets the currency mealbasisCode from third party mealbasisID.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party mealbasis code.</param>
        /// <returns>a mealbasisID code as integer, for the specified source  third party mealbasis code</returns>
        public async Task<string> GetMealBasisCodefromTPMealbasisIDAsync(string source, int thirdPartyMealBasis)
        {
            var mealbasis = string.Empty;

            var mealBases = await GetMealBasisAsync();

            var mealBasis = mealBases.FirstOrDefault(c => (c.Value.Source == source) && (c.Value.MealBasisID == thirdPartyMealBasis)).Value;

            if (mealBasis != null)
            {
                mealbasis = mealBasis.MealBasisCode;
            }

            return mealbasis;
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
                    async r => (await r.ReadAllAsync<MealBasisResult>()).ToDictionary(x => (x.Source, x.MealBasisCode), x => x.ToMealBasis()));
            }

            return await _cache.GetOrCreateAsync(CacheKey, cacheBuilder, 60);
        }

        public class MealBasisResult
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