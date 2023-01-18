namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using iVectorOne.Repositories;
    using iVectorOne.Search.Models;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///  Class responsible for building the extra factory
    /// </summary>
    public class ExtraFactory : IExtraFactory
    {
        /// <summary>The extra search repository</summary>
        private readonly IExtraSearchRepository _searchRepository;
        private readonly ITPSupport _tpSupport;

        /// <summary>The log writer</summary>
        private readonly ILogger<ExtraSearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="ExtraFactory" /> class.</summary>
        /// /// <param name="searchRepository">The extra search repository</param>
        /// <param name="logger">The log writer</param>
        public ExtraFactory(
            IExtraSearchRepository searchRepository,
            ILogger<ExtraSearchResponseFactory> logger,
            ITPSupport tpSupport)
        {
            _searchRepository = Ensure.IsNotNull(searchRepository, nameof(searchRepository));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _tpSupport = Ensure.IsNotNull(tpSupport, nameof(tpSupport));
        }

        public async Task<List<Extras>> CreateAsync(ExtraSearchDetails searchDetails, Account account)
        {
            return await _tpSupport.TPExtraLookupAsync(searchDetails);
        }
    }
}