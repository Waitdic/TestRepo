namespace ThirdParty
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Email;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    /// <summary>
    /// Third Party Property Search Base
    /// </summary>
    public class ThirdPartyPropertySearchRunner : IThirdPartyPropertySearchRunner
    {
        private readonly ILogger<ThirdPartyPropertySearchRunner> _logger;

        private readonly IEmailService _emailService;

        private readonly ISearchResultsProcessor _searchResultsProcessor;

        private readonly HttpClient _httpClient;

        public ThirdPartyPropertySearchRunner(
            ILogger<ThirdPartyPropertySearchRunner> logger,
            IEmailService emailService,
            ISearchResultsProcessor searchResultsProcessor,
            HttpClient httpClient)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _emailService = Ensure.IsNotNull(emailService, nameof(emailService));
            _searchResultsProcessor = Ensure.IsNotNull(searchResultsProcessor, nameof(searchResultsProcessor));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
        }

        public async Task SearchAsync(
            SearchDetails searchDetails,
            List<ResortSplit> resortSplits,
            IThirdPartySearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                // todo - email logs to
                // todo - request tracker, use mini profiler?
                StartTime = DateTime.Now; // needed for timeouts
                var taskList = new List<Task>();

                var requests = thirdPartySearch.BuildSearchRequests(searchDetails, resortSplits);

                foreach (var request in requests)
                {
                    request.Source = thirdPartySearch.Source;
                    request.LogFileName = "Search";
                    request.CreateLog = false; //TODO CS this should come from configuration
                    request.TimeoutInSeconds = RequestTimeOutSeconds(searchDetails);
                    request.UseGZip = UseGZip(searchDetails, thirdPartySearch.Source);

                    taskList.Add(_httpClient.SendAsync(request, _logger, cancellationTokenSource.Token));
                }

                await Task.WhenAll(taskList);

                if (!string.IsNullOrWhiteSpace(searchDetails.EmailLogsToAddress))
                {
                    foreach (var request in requests)
                    {
                        EmailSearchLogs(searchDetails.EmailLogsToAddress, thirdPartySearch.Source, request);
                    }
                }

                var transformedResponses = thirdPartySearch.TransformResponse(requests, searchDetails, resortSplits);

                await _searchResultsProcessor.ProcessTPResultsAsync(
                    transformedResponses,
                    thirdPartySearch.Source,
                    searchDetails,
                    resortSplits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        /// <summary>Gets or sets a value indicating whether [exclude non refundable].</summary>
        /// <value>
        /// <c>true</c> if [exclude non refundable]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool ExcludeNonRefundable { get; set; }

        /// <summary>Gets the current time taken in seconds.</summary>
        /// <value>The current time taken in seconds.</value>
        public int CurrentTimeTakenInSeconds
        {
            get
            {
                return (int)(DateTime.Now - this.StartTime).TotalSeconds;
            }
        }

        /// <summary>Gets or sets the start time.</summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; set; }

        /// <summary>Gets or sets the log collection.</summary>
        /// <value>The log collection.</value>
        public List<LogCollection> LogCollection { get; set; } = new List<LogCollection>();

        /// <summary>Gets or sets the request tracker.</summary>
        /// <value>The request tracker.</value>
        public IRequestTracker RequestTracker { get; set; } = null!;

        /// <summary>gets a unique request identifier.</summary>
        /// <param name="source">The source.</param>
        /// <param name="extraInfo">The o extra information.</param>
        /// <returns>
        ///   the unique request identifier as a string
        /// </returns>
        public static string UniqueRequestID(string source, object extraInfo)
        {
            var type = extraInfo.GetType();

            //// loop until we find a search extra helper or get to the base class
            while (!(type == null))
            {
                if (type == typeof(SearchExtraHelper))
                {
                    SearchExtraHelper searchHelper = (SearchExtraHelper)extraInfo;
                    string requestIdentifier = searchHelper.UniqueRequestID;

                    //// if the source is empty string then return the parent source
                    if (string.IsNullOrEmpty(requestIdentifier))
                    {
                        requestIdentifier = source;
                    }

                    return requestIdentifier;
                }
                else
                {
                    type = type.BaseType;
                }
            }

            return source;
        }

        /// <summary>Requests the time out seconds.</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        ///   The time out in seconds as an integer
        /// </returns>
        public int RequestTimeOutSeconds(SearchDetails searchDetails)
        {
            int timeOutSeconds = this.TimeoutSeconds(searchDetails) - this.CurrentTimeTakenInSeconds - 2;
            if (timeOutSeconds <= 0)
            {
                timeOutSeconds = 1;
            }

            return timeOutSeconds;
        }

        /// <summary>Timeouts the seconds.</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        ///   The seconds as an integer
        /// </returns>
        public int TimeoutSeconds(SearchDetails searchDetails)
        {
            return searchDetails.Settings.SearchTimeoutSeconds;
        }

        /// <summary>A boolean to decide if we want to compress the request</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private bool UseGZip(SearchDetails searchDetails, string source)
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