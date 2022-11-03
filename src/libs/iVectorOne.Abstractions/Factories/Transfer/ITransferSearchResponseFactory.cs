namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// produced transfer search responses
    /// </summary>
    public interface ITransferSearchResponseFactory
    {
        /// <summary>Creates a search response object</summary>
        /// <param name="searchDetails">The search details, which contains the results used for building the response</param>
        /// <param name="resortSplits">The resort splits, contains information looked up at start of search</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of response times</param>
        /// <returns>A property search responses</returns>
        Task<Response> CreateAsync(TransferSearchDetails searchDetails, List<SupplierResortSplit> resortSplits, IRequestTracker requestTracker);
    }
}