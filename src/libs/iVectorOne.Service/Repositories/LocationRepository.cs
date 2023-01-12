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

        public LocationRepository(ISql sql)
        {
            _sql = sql;
        }

        /// <inheritdoc />
        public async Task<List<Location>> GetAllLocations(string source)
        {
            var locations = await _sql.ReadSingleMappedAsync(
                 "select IVOLocationID as LocationID, Description  from IVOLocation where Source = @source",
                 async r => (await r.ReadAllAsync<Location>()).ToList(),
             new CommandSettings().WithParameters(new { source }));
            return locations;
        }
    }
}
