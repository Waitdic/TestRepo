namespace iVectorOne.Repositories
{
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    /// <seealso cref="ITransferSearchRepository" />
    public class TransferSearchRepository : ITransferSearchRepository
    {
        private readonly ISql _sql;
        private readonly ILogger<TransferSearchRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferSearchRepository"/> class.
        /// </summary>
        /// <param name="resortSplitFactory">The resort split factory.</param>
        public TransferSearchRepository(ISql sql, ILogger<TransferSearchRepository> logger)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task<LocationMapping> GetLocationMappingAsync(TransferSearchDetails searchDetails)
        {
            var results = await _sql.ReadSingleAsync<LocationMapping>(
                "Transfer_GetLocationMapping",
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

        public async Task AddLocations(string source, List<string> newLocations)
        {
            try
            {
                await _sql.ExecuteAsync(
                   "Transfer_AddLocations",
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
    }
}