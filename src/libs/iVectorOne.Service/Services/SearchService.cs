namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertySearch;
    using iVectorOne.Search;
    using iVectorOne.Search.Models;

    /// <summary>
    /// The main search service, responsible for doing all the searching
    /// </summary>
    /// <seealso cref="ISearchService" />
    public class SearchService : ISearchService
    {
        private readonly ISearchStoreService _searchStoreService;

        /// <summary>The search repository</summary>
        private readonly ISearchRepository _searchRepository;

        /// <summary>The search details factory</summary>
        private readonly ISearchDetailsFactory _searchDetailsFactory;

        /// <summary>The property search response factory</summary>
        private readonly IPropertySearchResponseFactory _propertySearchResponseFactory;

        /// <summary>A factory that returns the correct third party to search or book with, when given a source</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        private readonly IThirdPartyPropertySearchRunner _searchRunner;

        /// <summary>Initializes a new instance of the <see cref="SearchService" /> class.</summary>
        /// <param name="searchRepository">The search repository.</param>
        /// <param name="searchDetailsFactory">The search details factory.</param>
        /// <param name="propertySearchResponseFactory">The factory that produces the response using the results returned from the results processor</param>
        /// <param name="thirdPartyFactory">Takes in a source and return the third party search class</param>
        public SearchService(
            ISearchRepository searchRepository,
            ISearchDetailsFactory searchDetailsFactory,
            IPropertySearchResponseFactory propertySearchResponseFactory,
            IThirdPartyFactory thirdPartyFactory,
            IThirdPartyPropertySearchRunner searchRunner,
            ISearchStoreService searchStoreService)
        {
            _searchRepository = Ensure.IsNotNull(searchRepository, nameof(searchRepository));
            _searchDetailsFactory = Ensure.IsNotNull(searchDetailsFactory, nameof(searchDetailsFactory));
            _propertySearchResponseFactory = Ensure.IsNotNull(propertySearchResponseFactory, nameof(propertySearchResponseFactory));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _searchRunner = Ensure.IsNotNull(searchRunner, nameof(searchRunner));
            _searchStoreService = Ensure.IsNotNull(searchStoreService, nameof(searchStoreService));
        }

        /// <inheritdoc />
        public async Task<Response> SearchAsync(Request searchRequest, bool log, IRequestTracker requestTracker)
        {
            var totalTime = Stopwatch.StartNew();
            var stopwatch = Stopwatch.StartNew();

            // 1.Convert Request to Search details object
            var searchDetails = _searchDetailsFactory.Create(searchRequest, searchRequest.Account, log);
            searchDetails.SearchStoreItem.SearchDateAndTime = DateTime.Now;

            // 2.Check which suppliers to search
            var suppliers = searchRequest.Account.Configurations
                .Select(c => c.Supplier)
                .Where(s => !searchRequest.Suppliers.Any() || searchRequest.Suppliers.Contains(s))
                .ToList();

            // 3.Call db to get resort splits from central propery ids
            var resortSplits = await _searchRepository.GetResortSplitsAsync(
                string.Join(",", searchRequest.Properties),
                string.Join(",", suppliers),
                searchRequest.Account);

            var preprocessTime = (int)stopwatch.ElapsedMilliseconds;

            // 4.Run TP search
            await GetThirdPartySearchesAsync(resortSplits, searchDetails, requestTracker);

            stopwatch.Restart();

            // 5.Build response and return
            var response = await _propertySearchResponseFactory.CreateAsync(searchDetails, resortSplits, requestTracker);

            var postProcessTime = (int)stopwatch.ElapsedMilliseconds;

            searchDetails.SearchStoreItem.PropertiesReturned = response.PropertyResults.Count;
            searchDetails.SearchStoreItem.PreProcessTime = preprocessTime;
            searchDetails.SearchStoreItem.PostProcessTime += postProcessTime;
            searchDetails.SearchStoreItem.TotalTime = (int)totalTime.ElapsedMilliseconds;
            await _searchStoreService.AddAsync(searchDetails.SearchStoreItem);

            return response;
        }

        public async Task GetThirdPartySearchesAsync(List<SupplierResortSplit> supplierSplits, SearchDetails searchDetails, IRequestTracker requestTracker)
        {
            var taskList = new List<Task>();
            var cancellationTokenSource = new CancellationTokenSource();

            foreach (var split in supplierSplits)
            {
                var thirdPartySearch = _thirdPartyFactory.CreateSearchTPFromSupplier(split.Supplier);

                if (thirdPartySearch != null && !thirdPartySearch.SearchRestrictions(searchDetails, split.Supplier))
                {
                    // todo - request tracker
                    taskList.Add(
                        _searchRunner.SearchAsync(
                            searchDetails,
                            split,
                            thirdPartySearch,
                            cancellationTokenSource));
                }
            }

            await Task.WhenAll(taskList);
        }
    }
}