namespace iVectorOne.Search
{
    using Intuitive;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Processes search results
    /// </summary>
    /// <seealso cref="ITransferSearchResultsProcessor" />
    public class TransferSearchResultsProcessor : ITransferSearchResultsProcessor
    {
        /// <summary>The log writer</summary>
        private readonly ILogger<TransferSearchResultsProcessor> _logger;

        /// <summary>Initializes a new instance of the <see cref="SearchResultsProcessor" /> class.</summary>
        /// <param name="logWriter">The log writer</param>
        public TransferSearchResultsProcessor(
            ILogger<TransferSearchResultsProcessor> logger)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        /// <summary>
        /// Processes the Third party results asynchronous.
        /// </summary>
        /// <param name="results">The results XML.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <returns>
        /// A count of the results
        /// </returns>
        public int ProcessTPResultsAsync(TransformedTransferResultCollection results, string source, TransferSearchDetails searchDetails)
        {        
            int resultsCount = 0;

            try
            {
                //var searchResults = await _grouper.GroupPropertyResultsAsync(results, source, searchDetails, resortSplits);

                //if (searchResults.Any())
                //{
                resultsCount = results.ValidResults.Count;
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessTPResults error");
            }

            return resultsCount;
        }
    }
}
