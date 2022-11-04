namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    /// <seealso cref="ITransferSearchRepository" />
    public class TransferSearchRepository : ITransferSearchRepository
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
        public TransferSearchRepository(IResortSplitFactory resortSplitFactory, ISql sql)
        {
            _resortSplitFactory = Ensure.IsNotNull(resortSplitFactory, nameof(resortSplitFactory));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <inheritdoc/>
        public async Task<List<SupplierResortSplit>> GetResortSplitsAsync(string properties, string suppliers, Account account)
        {
            var results = await _sql.ReadAllAsync<CentralProperty>(
                "Search_GetThirdPartyData",
                new CommandSettings()
                    .IsStoredProcedure()
                    .WithParameters(new
                    {
                        centralPropertyIDs = properties,
                        sources = suppliers,
                        accountId = account.AccountID,
                    }));

            var resortSplits = _resortSplitFactory.Create(results.ToList());

            return resortSplits;
        }
    }
}