namespace iVectorOne.Services
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
    using iVectorOne.Constants;
    using iVectorOne.Search;

    /// <summary>
    /// The main transfer search service, responsible for doing all the searching
    /// </summary>
    /// <seealso cref="ThirdParty.Service.Services.ITransferSearchService" />
    public class TransferSearchService : ITransferSearchService
    {
        /// private readonly ISearchStoreService _searchStoreService;

        /// <summary>The search repository</summary>
        /// private readonly ISearchRepository _searchRepository;

        /// <summary>The transfer search details factory</summary>
        private readonly ITransferSearchDetailsFactory _searchDetailsFactory;

        /// <summary>The transfer search response factory</summary>
        private readonly ITransferSearchResponseFactory _transferSearchResponseFactory;

        /// <summary>A factory that returns the correct third party to search or book with, when given a source</summary>
        private readonly ITransferThirdPartyFactory _thirdPartyFactory;

        private readonly IThirdPartyTransferSearchRunner _searchRunner;


        /// <summary>Initializes a new instance of the <see cref="TransferSearchService" /> class.</summary>
        /// <param name="searchDetailsFactory">The search details factory.</param>
        /// <param name="transferSearchResponseFactory">The factory that produces the response using the results returned from the results processor</param>
        /// <param name="thirdPartyFactory">Takes in a source and return the third party search class</param>
        public TransferSearchService(
            ITransferSearchDetailsFactory searchDetailsFactory,
            ITransferSearchResponseFactory transferSearchResponseFactory,
            ITransferThirdPartyFactory thirdPartyFactory,
            IThirdPartyTransferSearchRunner searchRunner)
        {
            _searchDetailsFactory = Ensure.IsNotNull(searchDetailsFactory, nameof(searchDetailsFactory));
            _transferSearchResponseFactory = Ensure.IsNotNull(transferSearchResponseFactory, nameof(transferSearchResponseFactory));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _searchRunner = Ensure.IsNotNull(searchRunner, nameof(searchRunner));
        }

        /// <inheritdoc />
        public async Task<Response> SearchAsync(Request searchRequest, bool log, IRequestTracker requestTracker)
        {
            var totalTime = Stopwatch.StartNew();
            var stopwatch = Stopwatch.StartNew();
            //SearchStoreItem searchStoreItem = null!;

            try
            {
                // 1.Convert Request to Search details object
                var searchDetails = _searchDetailsFactory.Create(searchRequest, searchRequest.Account, log);
                // searchStoreItem = searchDetails.SearchStoreItem;
                //searchStoreItem.SearchDateAndTime = DateTime.Now;

                // 2.Check which suppliers to search
                //var suppliers = searchRequest.Account.Configurations
                //    .Select(c => c.Supplier)
                //    .Where(s => !searchRequest.Suppliers.Any() || searchRequest.Suppliers.Contains(s))
                //    .ToList();

                // 2.Check we want to search this supplier??
                var supplier = searchRequest.Account.Configurations
                    .Select(c => c.Supplier)
                    .Where(s => s == searchRequest.Supplier)
                    .FirstOrDefault();

                // 3.Call db to get resort splits from central propery ids
                var resortSplits = new List<SupplierResortSplit>();
                //var resortSplits = await _searchRepository.GetResortSplitsAsync(
                //    string.Join(",", searchRequest.Properties),
                //    string.Join(",", suppliers),
                //    searchRequest.Account);

                //searchStoreItem.PreProcessTime = (int)stopwatch.ElapsedMilliseconds;

                // 4.Run TP search
                await GetThirdPartySearchesAsync(supplier, searchDetails, requestTracker);

                stopwatch.Restart();

                // 5.Build response and return
                var response =
                    await _transferSearchResponseFactory.CreateAsync(searchDetails, resortSplits, requestTracker);

                //searchStoreItem.PostProcessTime += (int)stopwatch.ElapsedMilliseconds;
                //searchStoreItem.PropertiesReturned = response.TransferResults.Count;
                //searchStoreItem.Successful = true;

                return response;
            }
            finally
            {
                //if (searchStoreItem == null!)
                //{
                //    searchStoreItem = new SearchStoreItem
                //    {
                //        SearchStoreId = Guid.NewGuid(),
                //        SearchDateAndTime = DateTime.Now
                //    };

                //    if (searchRequest?.Account != null)
                //    {
                //        searchStoreItem.AccountName = searchRequest.Account.Login;
                //        searchStoreItem.AccountId = searchRequest.Account.AccountID;
                //        searchStoreItem.System = searchRequest.Account.Environment.ToString();
                //    }
                //}

                //searchStoreItem.TotalTime = (int)totalTime.ElapsedMilliseconds;

                //_ = _searchStoreService.AddAsync(searchStoreItem);
            }
        }

        public async Task GetThirdPartySearchesAsync(string supplier, TransferSearchDetails searchDetails, IRequestTracker requestTracker)
        {
            var taskList = new List<Task>();
            var cancellationTokenSource = new CancellationTokenSource();

            //foreach (var split in supplierSplits)
            //{
            //    var thirdPartySearch = _thirdPartyFactory.CreateSearchTPFromSupplier(split.Supplier);

            //    if (thirdPartySearch != null && !thirdPartySearch.SearchRestrictions(searchDetails, split.Supplier))
            //    {
            //        // todo - request tracker
            //        taskList.Add(
            //            _searchRunner.SearchAsync(
            //                searchDetails,
            //                split,
            //                thirdPartySearch,
            //                cancellationTokenSource));
            //    }
            //}

            var thirdPartySearch = _thirdPartyFactory.CreateSearchTPFromSupplier(ThirdParties.NULLTESTTRANSFERSUPPLIER);

            if (thirdPartySearch != null && !thirdPartySearch.SearchRestrictions(searchDetails, ThirdParties.NULLTESTTRANSFERSUPPLIER))
            {
                // todo - request tracker
                taskList.Add(
                    _searchRunner.SearchAsync(
                        searchDetails,
                        thirdPartySearch,
                        cancellationTokenSource));
            }

            await Task.WhenAll(taskList);
        }
    }
}