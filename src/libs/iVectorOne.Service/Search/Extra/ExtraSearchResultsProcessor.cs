namespace iVectorOne.Search
{
    using Intuitive;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Search.Results.Models.Extra;
    using iVectorOne.Utility;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Processes search results
    /// </summary>
    /// <seealso cref="IExtraSearchResultsProcessor" />
    public class ExtraSearchResultsProcessor : IExtraSearchResultsProcessor
    {
        /// <summary>The currency look up repository</summary>
        private readonly ICurrencyLookupRepository _currencyRepository;

        /// <summary>The log writer</summary>
        private readonly ILogger<ExtraSearchResultsProcessor> _logger;

        /// <summary>Initializes a new instance of the <see cref="SearchResultsProcessor" /> class.</summary>
        /// /// <param name="currencyRepository">The currency look up repository</param>
        /// <param name="logWriter">The log writer</param>
        public ExtraSearchResultsProcessor(
            ICurrencyLookupRepository currencyRepository,
            ILogger<ExtraSearchResultsProcessor> logger)
        {
            _currencyRepository = Ensure.IsNotNull(currencyRepository, nameof(currencyRepository));
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
        public async Task<int> ProcessTPResultsAsync(TransformedExtraResultCollection results, ExtraSearchDetails searchDetails)
        {        
            int resultsCount = 0;

            try
            {
                foreach (var result in results.ValidResults)
                {
                    var searchResult = new ExtraSearchResult()
                    {
                        TPSessionID = result.TPSessionID,
                        SupplierReference = result.SupplierReference,
                        ExtraName = result.ExtraName,
                        ExtraCategory = result.ExtraCategory,
                        UseDate = result.UseDate,
                        UseTime= result.UseTime,
                        EndDate= result.EndDate,
                        EndTime= result.EndTime,
                        CurrencyID = await ProcessorHelpers.GetISOCurrencyID(searchDetails.Source, result.CurrencyCode, searchDetails.AccountID, _currencyRepository),
                        Cost = result.Cost,
                        AdditionalDetails= result.AdditionalDetails,
                       
                    };

                    searchDetails.Results.SearchResults.Add(searchResult);
                }

                resultsCount = results.ValidResults.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessTPResults error");
            }

            return resultsCount;
        }
    }
}
