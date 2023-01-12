namespace iVectorOne.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using Intuitive;

    /// <summary>
    /// Processes search results
    /// </summary>
    /// <seealso cref="Search.ISearchResultsProcessor" />
    public class SearchResultsProcessor : ISearchResultsProcessor
    {
        /// <summary>The de-duplicator</summary>
        private readonly IResultDeduper _deduper;

        /// <summary>The group results processor</summary>
        private readonly IGroupResultsProcessor _grouper;

        /// <summary>The log writer</summary>
        private readonly ILogger<SearchResultsProcessor> _logger;

        /// <summary>Initializes a new instance of the <see cref="SearchResultsProcessor" /> class.</summary>
        /// <param name="deduper">The duplicator.</param>
        /// <param name="grouper">The grouper.</param>
        /// <param name="logger">The log writer</param>
        /// <param name="serializer">The xml serializer</param>
        public SearchResultsProcessor(
            IResultDeduper deduper,
            IGroupResultsProcessor grouper,
            ILogger<SearchResultsProcessor> logger)
        {
            _deduper = Ensure.IsNotNull(deduper, nameof(deduper));
            _grouper = Ensure.IsNotNull(grouper, nameof(grouper));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        /// <summary>
        /// Processes the Third party results asynchronous.
        /// </summary>
        /// <param name="results">The results XML.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <returns>
        /// A count of the results
        /// </returns>
        public async Task<int> ProcessTPResultsAsync(TransformedResultCollection results, string source, SearchDetails searchDetails, IEnumerable<IResortSplit> resortSplits)
        {
            int resultsCount = 0;

            try
            {
                var searchResults = await _grouper.GroupPropertyResultsAsync(results, source, searchDetails, resortSplits);

                if (searchResults.Any())
                {
                    resultsCount = await _deduper.DedupeResultsAsync(searchResults, searchDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessTPResults error");
            }

            return resultsCount;
        }
    }
}
