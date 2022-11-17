namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    /// <seealso cref="ITransferSearchRepository" />
    public class TransferSearchRepository : ITransferSearchRepository
    {
        private readonly ISql _sql;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferSearchRepository"/> class.
        /// </summary>
        /// <param name="resortSplitFactory">The resort split factory.</param>
        public TransferSearchRepository(ISql sql)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
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
    }
}