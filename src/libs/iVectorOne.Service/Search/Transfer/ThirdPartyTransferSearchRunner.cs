namespace iVectorOne
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Email;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.SearchStore;
    using iVectorOne.Search;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;

    /// <summary>
    /// Third Party Transfer Search Runner
    /// </summary>
    public class ThirdPartyTransferSearchRunner : IThirdPartyTransferSearchRunner
    {
        private readonly ISearchStoreService _searchStoreService;
        private readonly ILogger<ThirdPartyTransferSearchRunner> _logger;

        private readonly IEmailService _emailService;

        private readonly ITransferSearchResultsProcessor _searchResultsProcessor;

        private readonly HttpClient _httpClient;

        public ThirdPartyTransferSearchRunner(
            ILogger<ThirdPartyTransferSearchRunner> logger,
            IEmailService emailService,
            ITransferSearchResultsProcessor searchResultsProcessor,
            HttpClient httpClient,
            ISearchStoreService searchStoreService)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _emailService = Ensure.IsNotNull(emailService, nameof(emailService));
            _searchResultsProcessor = Ensure.IsNotNull(searchResultsProcessor, nameof(searchResultsProcessor));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _searchStoreService = Ensure.IsNotNull(searchStoreService, nameof(searchStoreService)); ;
        }

        /// <summary>Gets the current time taken in seconds.</summary>
        public int CurrentTimeTakenInSeconds
            => (int)(DateTime.Now - this.StartTime).TotalSeconds;

        /// <summary>Gets or sets the start time.</summary>
        public DateTime StartTime { get; set; }

        /// <summary>Gets or sets the request tracker.</summary>
        public IRequestTracker RequestTracker { get; set; } = null!;

        /// <inheritdoc />
        public async Task SearchAsync(
            TransferSearchDetails searchDetails,
            LocationMapping locationMapping,
            IThirdPartyTransferSearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                // todo - request tracker, use mini profiler?
                StartTime = DateTime.Now; // needed for timeouts
                var stopwatch = Stopwatch.StartNew();

                var taskList = new List<Task>();
                string source = searchDetails.Source;
                //var resortSplits = supplierResortSplit.ResortSplits;

                var requests = await thirdPartySearch.BuildSearchRequestsAsync(searchDetails, locationMapping);

                var preprocessTime = (int)stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                foreach (var request in requests)
                {
                    request.Source = source;
                    request.LogFileName = "Search";
                    request.CreateLog = false; //TODO CS this should come from configuration

                    //uncomment after third party configs setup for transfer third parties
                    //request.TimeoutInSeconds = RequestTimeOutSeconds(searchDetails, source); 
                    //request.UseGZip = UseGZip(searchDetails, source);

                    taskList.Add(_httpClient.SendAsync(request, _logger, cancellationTokenSource.Token));
                }

                await Task.WhenAll(taskList);

                var supplierTime = (int)stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                if (requests.Any())
                {
                    if (!string.IsNullOrWhiteSpace(searchDetails.EmailLogsToAddress))
                    {
                        foreach (var request in requests)
                        {
                            EmailSearchLogs(searchDetails.EmailLogsToAddress, source, request);
                        }
                    }

                    var transformedResponses = thirdPartySearch.TransformResponse(requests, searchDetails, locationMapping);

                    var postProcessTime = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    var resultsCount = _searchResultsProcessor.ProcessTPResultsAsync(transformedResponses, searchDetails);

                    //var searchStoreSupplierItem = new SearchStoreSupplierItem
                    //{
                    //    SearchStoreId = searchDetails.SearchStoreItem.SearchStoreId,
                    //    AccountName = searchDetails.SearchStoreItem.AccountName,
                    //    AccountId = searchDetails.SearchStoreItem.AccountId,
                    //    System = searchDetails.SearchStoreItem.System,
                    //    SupplierName = source,
                    //    Successful = requests.All(r => r.Success),
                    //    Timeout = requests.Any(r => r.TimeOut),
                    //    SearchDateAndTime = searchDetails.SearchStoreItem.SearchDateAndTime,
                    //    PropertiesRequested = resortSplits.Sum(r => r.Hotels.Count),
                    //    PropertiesReturned = resultsCount,
                    //    PreProcessTime = preprocessTime,
                    //    SupplierTime = supplierTime,
                    //    DedupeTime = dedupeTime,
                    //    PostProcessTime = postProcessTime,
                    //    TotalTime = preprocessTime + supplierTime + dedupeTime + postProcessTime
                    //};

                    //searchDetails.SearchStoreItem.MaxSupplierTime = Math.Max(
                    //    searchStoreSupplierItem.SupplierTime,
                    //    searchDetails.SearchStoreItem.MaxSupplierTime);

                    //searchDetails.SearchStoreItem.MaxDedupeTime = Math.Max(
                    //    searchStoreSupplierItem.DedupeTime,
                    //    searchDetails.SearchStoreItem.MaxDedupeTime);

                    //searchDetails.SearchStoreItem.PostProcessTime += dedupeTime;

                    //_ = _searchStoreService.AddAsync(searchStoreSupplierItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        /// <summary>Requests the time out seconds.</summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   The time out in seconds as an integer
        /// </returns>
        public int RequestTimeOutSeconds(TransferSearchDetails searchDetails, string source)
        {
            int timeOutSeconds = this.TimeoutSeconds(searchDetails, source) - this.CurrentTimeTakenInSeconds - 2;
            if (timeOutSeconds <= 0)
            {
                timeOutSeconds = 1;
            }

            return timeOutSeconds;
        }

        /// <summary>Timeouts the seconds.</summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   The seconds as an integer
        /// </returns>
        public int TimeoutSeconds(TransferSearchDetails searchDetails, string source)
        {
            var configuration = searchDetails.ThirdPartyConfigurations.FirstOrDefault(c => c.Supplier == source);
             
            return configuration.Configurations.ContainsKey("SearchTimeout") ? 
                configuration.Configurations["SearchTimeout"].ToSafeInt() :
                searchDetails.Settings.SearchTimeoutSeconds;
        }

        /// <summary>A boolean to decide if we want to compress the request</summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <returns>The value of the use gzip setting</returns>
        private bool UseGZip(TransferSearchDetails searchDetails, string source)
        {
            var configuration = searchDetails.ThirdPartyConfigurations.FirstOrDefault(c => c.Supplier == source);

            var useGZip = false;

            if (searchDetails != null)
            {
                if (configuration.Configurations.ContainsKey("UseGZip"))
                {
                    useGZip = configuration.Configurations["UseGZip"].ToSafeBoolean();
                }
            }

            return useGZip;
        }

        /// <summary>Emails the search logs.</summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="request">The web request.</param>
        private void EmailSearchLogs(string emailAddress, string provider, Request request)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("Request").AppendLine().AppendLine();
                sb.Append(request.RequestLog).AppendLine().AppendLine();
                sb.Append("Response").AppendLine().AppendLine();
                sb.Append(request.ResponseLog);

                var email = new Email()
                {
                    EmailTo = emailAddress,
                    From = $"{provider} Search Logs",
                    FromEmail = "searchlogs@intuitivesystems.co.uk",
                    Subject = $"Search Logs - {provider} {DateTime.Now}",
                    Body = sb.ToString()
                };

                _emailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error emailing search logs");
            }
        }
    }
}