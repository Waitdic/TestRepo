namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Repositories;
    using iVectorOne.Search.Models;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    /// <summary>
    ///  Class responsible for building the transfer location mapping
    /// </summary>
    public class TransferLocationMappingFactory : ITransferLocationMappingFactory
    {
        /// <summary>The transfer search repository</summary>
        private readonly ITransferSearchRepository _searchRepository;
        private readonly ITPSupport _tpSupport;

        /// <summary>The log writer</summary>
        private readonly ILogger<TransferSearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="TransferLocationMappingFactory" /> class.</summary>
        /// /// <param name="searchRepository">The transfer search repository</param>
        /// <param name="logger">The log writer</param>
        public TransferLocationMappingFactory(
            ITransferSearchRepository searchRepository,
            ILogger<TransferSearchResponseFactory> logger,
            ITPSupport tpSupport)
        {
            _searchRepository = Ensure.IsNotNull(searchRepository, nameof(searchRepository));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _tpSupport = Ensure.IsNotNull(tpSupport, nameof(tpSupport));
        }

        public async Task<LocationMapping> CreateAsync(TransferSearchDetails searchDetails, Account account)
        {
            return await _tpSupport.TPLocationLookupAsync(searchDetails);
        }
    }
}