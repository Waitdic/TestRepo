namespace iVectorOne.Search
{
    using Intuitive;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Utility;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Processes search results
    /// </summary>
    /// <seealso cref="ITransferSearchResultsProcessor" />
    public class TransferSearchResultsProcessor : ITransferSearchResultsProcessor
    {
        /// <summary>The currency look up repository</summary>
        private readonly ICurrencyLookupRepository _currencyRepository;

        /// <summary>The log writer</summary>
        private readonly ILogger<TransferSearchResultsProcessor> _logger;

        /// <summary>Initializes a new instance of the <see cref="SearchResultsProcessor" /> class.</summary>
        /// /// <param name="currencyRepository">The currency look up repository</param>
        /// <param name="logWriter">The log writer</param>
        public TransferSearchResultsProcessor(
            ICurrencyLookupRepository currencyRepository,
            ILogger<TransferSearchResultsProcessor> logger)
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
        public async Task<int> ProcessTPResultsAsync(TransformedTransferResultCollection results, TransferSearchDetails searchDetails)
        {        
            int resultsCount = 0;

            try
            {
                foreach (var result in results.ValidResults)
                {
                    var searchResult = new TransferSearchResult()
                    {
                        TPSessionID = result.TPSessionID,
                        SupplierReference = result.SupplierReference,
                        TransferVehicle = result.TransferVehicle,
                        ReturnTime = result.ReturnTime,
                        VehicleCost = result.VehicleCost,
                        AdultCost = result.AdultCost,
                        ChildCost = result.ChildCost,
                        CurrencyID = await ProcessorHelpers.GetISOCurrencyID(searchDetails.Source, result.CurrencyCode, searchDetails.AccountID, _currencyRepository),
                        VehicleQuantity = result.VehicleQuantity,
                        Cost = result.Cost,
                        BuyingChannelCost = result.BuyingChannelCost,
                        OutboundInformation = result.OutboundInformation,
                        ReturnInformation = result.ReturnInformation,
                        OutboundCost = result.OutboundCost,
                        ReturnCost = result.ReturnCost,
                        OutboundXML = result.OutboundXML,
                        ReturnXML = result.ReturnXML, 
                        OutboundTransferMinutes = result.OutboundTransferMinutes,
                        ReturnTransferMinutes = result.ReturnTransferMinutes,
                        OnRequest = result.OnRequest,
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
