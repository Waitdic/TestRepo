namespace ThirdParty
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    /// <summary>
    /// Third Party Property Search Base
    /// </summary>
    public abstract class ThirdPartyPropertySearchBase
    {
        private readonly ILogger _logger;

        public ThirdPartyPropertySearchBase(ILogger logger)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task SearchAsync(
            SearchDetails searchDetails,
            List<ResortSplit> resortSplits,
            CancellationTokenSource cancellationTokenSource,
            HttpClient httpClient)
        {
            try
            {
                // todo - email logs to
                // todo - request tracker, use mini profiler?
                StartTime = DateTime.Now; // needed for timeouts
                var taskList = new List<Task>();

                var requests = BuildSearchRequests(searchDetails, resortSplits, false);

                foreach (var request in requests)
                {
                    request.Source = Source;
                    request.LogFileName = "Search";
                    request.CreateLog = false; //TODO CS this should come from configuration
                    request.TimeoutInSeconds = RequestTimeOutSeconds(searchDetails);
                    request.UseGZip = UseGZip(searchDetails);

                    taskList.Add(httpClient.SendAsync(request, _logger, cancellationTokenSource.Token));
                }

                await Task.WhenAll(taskList);

                var tranformedResponses = TransformResponse(requests, searchDetails, resortSplits);

                await SearchResultsProcessor.ProcessTPResultsAsync(
                    tranformedResponses,
                    Source,
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

        /// <summary>Gets or sets the search results processor.</summary>
        /// <value>The search results processor.</value>
        public ISearchResultsProcessor SearchResultsProcessor { get; set; } = null!;

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

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public abstract string Source { get; }

        /// <summary>
        /// Gets a value indicating whether [SQL request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [SQL request]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool SqlRequest { get; }

        /// <summary>
        /// Gets a value indicating whether [supports non refundable tagging].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supports non refundable tagging]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsNonRefundableTagging { get; } = true;

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

        /// <summary>
        /// Builds the search requests.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <param name="saveLogs">if set to <c>true</c> [b save logs].</param>
        /// <returns>A list of request</returns>
        public abstract List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs);

        /// <summary>
        /// Transforms the response.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>an XML document</returns>
        public abstract TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits);

        /// <summary>
        /// '''     Check if there are any search restrictions for the third party.
        /// '''     For example; the third party can not perform multi-room bookings.
        /// '''
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        /// If there any search restrictions for the third party.
        /// </returns>
        /// '''
        /// '''
        public abstract bool SearchRestrictions(SearchDetails searchDetails);

        /// <summary>
        /// Responses the has exceptions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>a boolean if the response has an exception</returns>
        public abstract bool ResponseHasExceptions(Request request);

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
        private bool UseGZip(SearchDetails searchDetails)
        {
            var configuration = searchDetails.ThirdPartyConfigurations.FirstOrDefault(c => c.Supplier == this.Source);

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

        /// <summary>A boolean to decide if we want to compress the request</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private string SMTPHost(SearchDetails searchDetails)
        {
            var configuration = searchDetails.ThirdPartyConfigurations.FirstOrDefault(c => c.Supplier == this.Source);

            var smtpHost = string.Empty;

            if (searchDetails != null)
            {
                if (configuration.Configurations.ContainsKey("SMTPHost"))
                {
                    smtpHost = configuration.Configurations["SMTPHost"].ToSafeString();
                }
            }

            return smtpHost;
        }
    }
}