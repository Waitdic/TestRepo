using Intuitive;
using Intuitive.Data;
namespace iVectorOne.Repositories.Extra
{
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ExtraSearchRepository : IExtraSearchRepository
    {
        private readonly ISql _sql;
        private readonly ILogger<ExtraSearchRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraSearchRepository"/> class.
      
        public ExtraSearchRepository(ISql sql, ILogger<ExtraSearchRepository> logger)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task AddLocations(string source, List<string> newLocations)
        {
            try
            {
                await _sql.ExecuteAsync(
                   "Extra_AddLocations",
                   new CommandSettings()
                       .IsStoredProcedure()
                       .WithParameters(new
                       {
                           source = source,
                           locations = string.Join(",", newLocations)
                       }));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occured during adding the new locations.");
            }
        }

        public async Task<LocationMapping> GetLocationMappingAsync(ExtraSearchDetails searchDetails)
        {
            var results = await _sql.ReadSingleAsync<LocationMapping>(
              "Extra_GetLocationMapping",
              new CommandSettings()
                  .IsStoredProcedure()
                  .WithParameters(new
                  {
                      departureLocationID = searchDetails.DepartureLocationId,
                      arrivalLocationID = searchDetails.ArrivalLocationId,
                      source = searchDetails.Source,
                  }));

            return results;
        }
    }
}
