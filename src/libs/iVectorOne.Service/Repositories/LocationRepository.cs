namespace iVectorOne.Repositories
{
    using Intuitive.Data;
    using iVectorOne.SDK.V2.LocationContent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Caching.Memory;

    public class LocationRepository : ILocationRepository
    {
        private const int _timeout = 2;

        private readonly ISql _sql;
        private readonly IMemoryCache _cache;

        public LocationRepository(ISql sql,IMemoryCache cache)
        {
            _sql = sql;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<List<Location>> GetAllLocations(string source)
        {
            string cacheKey = "TPLocationContentLookup_" + source;

            async Task<List<Location>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select IVOLocationID as LocationID, Description  from IVOLocation where Source = @source",
                    async r => (await r.ReadAllAsync<Location>()).ToList(),
                new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }
    }
}
