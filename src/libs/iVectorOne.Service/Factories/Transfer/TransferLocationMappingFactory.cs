namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///  Class responsible for building the transfer location mapping
    /// </summary>
    public class TransferLocationMappingFactory : ITransferLocationMappingFactory
    {
        /// <summary>The transfer search repository</summary>
        private readonly ITransferSearchRepository _searchRepository;

        /// <summary>The log writer</summary>
        private readonly ILogger<TransferSearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="TransferLocationMappingFactory" /> class.</summary>
        /// /// <param name="searchRepository">The transfer search repository</param>
        /// <param name="logger">The log writer</param>
        public TransferLocationMappingFactory(
            ITransferSearchRepository searchRepository,
            ILogger<TransferSearchResponseFactory> logger)
        {
            _searchRepository = Ensure.IsNotNull(searchRepository, nameof(searchRepository));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        /// <summary>Creates the location mapping.</summary>
        /// <param name="searchDetails">The search details, used to retrieve information about the search</param>
        /// <param name="account">The account.....</param>
        /// <returns>A transfer location map</returns>
        public LocationMapping Create(TransferSearchDetails searchDetails, Account account)
        {
            return new LocationMapping();
        }
    }
}