namespace ThirdParty.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using ThirdParty.Models;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    /// <seealso cref="ISearchRepository" />
    public class SearchRepository : ISearchRepository
    {
        /// <summary>
        /// The resort split factory
        /// </summary>
        private readonly IResortSplitFactory _resortSplitFactory;

        private readonly ISql _sql;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRepository"/> class.
        /// </summary>
        /// <param name="resortSplitFactory">The resort split factory.</param>
        public SearchRepository(IResortSplitFactory resortSplitFactory, ISql sql)
        {
            _resortSplitFactory = Ensure.IsNotNull(resortSplitFactory, nameof(resortSplitFactory));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <summary>
        /// Takes in a comma separated list of central property identifiers and suppliers, 
        /// calls the database and returns a list of resort splits
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="suppliers">The suppliers.</param>
        /// <returns>a list or resort splits</returns>
        public async Task<List<SupplierResortSplit>> GetResortSplitsAsync(string properties, string suppliers)
        {
            var results = await _sql.ReadAllAsync<CentralProperty>(
                "Search_GetThirdPartyData",
                new CommandSettings()
                    .IsStoredProcedure()
                    .WithParameters(new
                    {
                        sCentralPropertyIDs = properties,
                        sSources = suppliers
                    }));

            var resortSplits = _resortSplitFactory.Create(results.ToList());

            return resortSplits;
        }
    }
}