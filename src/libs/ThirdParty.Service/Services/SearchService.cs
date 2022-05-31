namespace ThirdParty.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Intuitive;
    using ThirdParty;
    using ThirdParty.Factories;
    using ThirdParty.Models;
    using ThirdParty.Repositories;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Search.Models;

    /// <summary>
    /// The main search service, responsible for doing all the searching
    /// </summary>
    /// <seealso cref="ISearchService" />
    public class SearchService : ISearchService
    {
        /// <summary>The search repository</summary>
        private readonly ISearchRepository _searchRepository;

        /// <summary>The search details factory</summary>
        private readonly ISearchDetailsFactory _searchDetailsFactory;

        /// <summary>The search results processor</summary>
        private readonly ISearchResultsProcessor _searchResultsProcessor;

        /// <summary>The property search response factory</summary>
        private readonly IPropertySearchResponseFactory _propertySearchResponseFactory;

        /// <summary>A factory that returns the correct third party to search or book with, when given a source</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        private readonly HttpClient _httpClient;

        /// <summary>Initializes a new instance of the <see cref="SearchService" /> class.</summary>
        /// <param name="searchRepository">The search repository.</param>
        /// <param name="searchDetailsFactory">The search details factory.</param>
        /// <param name="searchResultsProcessor">The search results processor</param>
        /// <param name="propertySearchResponseFactory">The factory that produces the response using the results returned from the results processor</param>
        /// <param name="thirdPartyFactory">Takes in a source and return the third party search class</param>
        public SearchService(
            ISearchRepository searchRepository,
            ISearchDetailsFactory searchDetailsFactory,
            ISearchResultsProcessor searchResultsProcessor,
            IPropertySearchResponseFactory propertySearchResponseFactory,
            IThirdPartyFactory thirdPartyFactory,
            HttpClient httpClient)
        {
            _searchRepository = Ensure.IsNotNull(searchRepository, nameof(searchRepository));
            _searchDetailsFactory = Ensure.IsNotNull(searchDetailsFactory, nameof(searchDetailsFactory));
            _searchResultsProcessor = Ensure.IsNotNull(searchResultsProcessor, nameof(searchResultsProcessor));
            _propertySearchResponseFactory = Ensure.IsNotNull(propertySearchResponseFactory, nameof(propertySearchResponseFactory));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
        }

        public async Task<Response> SearchAsync(Request searchRequest, User user, bool log, IRequestTracker requestTracker)
        {
            // 1.Convert Request to Search details object
            var searchDetails = _searchDetailsFactory.Create(searchRequest, user, log);

            // 2.Check which suppliers to search
            var suppliers = searchRequest.Suppliers.Any() ?
                searchRequest.Suppliers :
                user.Configurations.Select(c => c.Supplier).ToList();

            // 3.Call db to get resort splits from central propery ids
            var resortSplits = await _searchRepository.GetResortSplitsAsync(string.Join(",", searchDetails.PropertyReferenceIDs), string.Join(",", suppliers));

            // 4.Run TP search
            await GetThirdPartySearchesAsync(resortSplits, searchDetails, requestTracker);

            // 5.Build response and return
            var response = await _propertySearchResponseFactory.CreateAsync(searchDetails, resortSplits, requestTracker);

            return response;
        }

        public async Task GetThirdPartySearchesAsync(List<SupplierResortSplit> supplierSplits, SearchDetails searchDetails, IRequestTracker requestTracker)
        {
            var taskList = new List<Task>();
            var cancellationTokenSource = new CancellationTokenSource();

            foreach (var split in supplierSplits)
            {
                var thirdPartySearch = _thirdPartyFactory.CreateSearchTPFromSupplier(split.Supplier);

                if (thirdPartySearch != null && !thirdPartySearch.SearchRestrictions(searchDetails))
                {
                    thirdPartySearch.SearchResultsProcessor = _searchResultsProcessor;

                    thirdPartySearch.RequestTracker = requestTracker;

                    taskList.Add(thirdPartySearch.SearchAsync(searchDetails, split.ResortSplits, cancellationTokenSource, _httpClient));
                }
            }

            await Task.WhenAll(taskList);
        }
    }
}