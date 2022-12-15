namespace iVectorOne.Services.Transfer
{
    using Intuitive;
    using iVectorOne.Factories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;
    using System.Diagnostics;
    using iVectorOne.Models;
    using System.Threading;
    using iVectorOne.Search;
    using iVectorOne.Interfaces;
    using iVectorOne.Models.SearchStore;
    using System;
    using iVectorOne.Repositories;

    /// <summary>
    /// The main transfer search service, responsible for doing all the searching
    /// </summary>
    /// <seealso cref="ThirdParty.Service.Services.ITransferSearchService" />
    public class SearchService : ISearchService
    {
        private readonly ISearchStoreService _searchStoreService;

        /// <summary>The search repository</summary>
        private readonly ISearchRepository _searchRepository;

        /// <summary>The search repository</summary>
        private readonly ITransferLocationMappingFactory _locationMappingFactory;

        /// <summary>The transfer search details factory</summary>
        private readonly ITransferSearchDetailsFactory _searchDetailsFactory;

        /// <summary>The transfer search response factory</summary>
        private readonly ITransferSearchResponseFactory _transferSearchResponseFactory;

        /// <summary>A factory that returns the correct third party to search or book with, when given a source</summary>
        private readonly ITransferThirdPartyFactory _thirdPartyFactory;

        private readonly IThirdPartyTransferSearchRunner _searchRunner;

        /// <summary>Initializes a new instance of the <see cref="TransferSearchService" /> class.</summary>
        /// <param name="locationMappingFactory">The location mapping factory.</param>
        /// <param name="searchDetailsFactory">The search details factory.</param>
        /// <param name="transferSearchResponseFactory">The factory that produces the response using the results returned from the results processor</param>
        /// <param name="thirdPartyFactory">Takes in a source and return the third party search class</param>
        /// <param name="searchStoreService">The service to store the search </param>
        public SearchService(
            ITransferLocationMappingFactory locationMappingFactory,
            ITransferSearchDetailsFactory searchDetailsFactory,
            ITransferSearchResponseFactory transferSearchResponseFactory,
            ITransferThirdPartyFactory thirdPartyFactory,
            IThirdPartyTransferSearchRunner searchRunner,
            ISearchStoreService searchStoreService,
            ISearchRepository searchRepository)
        {
            _locationMappingFactory = Ensure.IsNotNull(locationMappingFactory, nameof(locationMappingFactory));
            _searchDetailsFactory = Ensure.IsNotNull(searchDetailsFactory, nameof(searchDetailsFactory));
            _transferSearchResponseFactory = Ensure.IsNotNull(transferSearchResponseFactory, nameof(transferSearchResponseFactory));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _searchRunner = Ensure.IsNotNull(searchRunner, nameof(searchRunner));
            _searchStoreService = Ensure.IsNotNull(searchStoreService, nameof(searchStoreService));
            _searchRepository = Ensure.IsNotNull(searchRepository, nameof(searchRepository));
        }

        /// <inheritdoc />
        public async Task<Response> SearchAsync(Request searchRequest, bool log, IRequestTracker requestTracker)
        {
            var totalTime = Stopwatch.StartNew();
            var stopwatch = Stopwatch.StartNew();
            TransferSearchStoreItem searchStoreItem = null!;

            var locationMapping = new LocationMapping();
            var taskList = new List<Task>();
            var cancellationTokenSource = new CancellationTokenSource();
            Response response;
            try
            {
                var searchDetails = _searchDetailsFactory.Create(searchRequest, searchRequest.Account, log);
                searchStoreItem = searchDetails.SearchStoreItem;
                searchStoreItem.SearchDateAndTime = DateTime.Now;
                searchStoreItem.PreProcessTime = (int)stopwatch.ElapsedMilliseconds;

                var thirdPartySearch = _thirdPartyFactory.CreateSearchTPFromSupplier(searchDetails.Source);

                if (thirdPartySearch != null
                    && !thirdPartySearch.SearchRestrictions(searchDetails)
                    && AccountHasSupplier(searchRequest))
                {
                    searchDetails.Source = (thirdPartySearch as ISingleSource)!.Source;

                    locationMapping = await _locationMappingFactory.CreateAsync(searchDetails, searchRequest.Account);

                    if (!thirdPartySearch.ValidateSettings(searchDetails))
                    {
                        response = new Response()
                        {
                            Warnings = searchDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
                        };
                        return response;
                    }

                    taskList.Add(
                        _searchRunner.SearchAsync(
                            searchDetails,
                            locationMapping,
                            thirdPartySearch,
                            cancellationTokenSource));
                }

                await Task.WhenAll(taskList);

                stopwatch.Restart();

                response = await _transferSearchResponseFactory.CreateAsync(searchDetails, locationMapping, requestTracker);

                searchStoreItem.PostProcessTime += (int)stopwatch.ElapsedMilliseconds;
                searchStoreItem.ResultsReturned = response.TransferResults.Count;
                searchStoreItem.Successful = true;

                return response;
            }
            finally
            {
                if (searchStoreItem == null!)
                {
                    searchStoreItem = new TransferSearchStoreItem
                    {
                        TransferSearchStoreId = Guid.NewGuid(),
                        SearchDateAndTime = DateTime.Now
                    };

                    if (searchRequest?.Account != null)
                    {
                        searchStoreItem.AccountName = searchRequest.Account.Login;
                        searchStoreItem.AccountId = searchRequest.Account.AccountID;
                        searchStoreItem.System = searchRequest.Account.Environment.ToString();
                    }
                }

                searchStoreItem.TotalTime = (int)totalTime.ElapsedMilliseconds;

                _ = _searchStoreService.AddAsync(searchStoreItem);
            }
        }

        private bool AccountHasSupplier(Request searchRequest)
            => searchRequest.Account.Configurations.Where(c => c.Supplier.ToLower() == searchRequest.Supplier.ToLower()).Any();
    }
}