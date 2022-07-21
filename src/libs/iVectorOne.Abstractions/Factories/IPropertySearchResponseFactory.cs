namespace ThirdParty.Factories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Search.Models;

    /// <summary>
    /// produced property search responses
    /// </summary>
    public interface IPropertySearchResponseFactory
    {
        /// <summary>Creates a search response object</summary>
        /// <param name="searchDetails">The search details, which contains the results used for building the response</param>
        /// <param name="resortSplits">The resort splits, contains information looked up at start of search</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of response times</param>
        /// <returns>A property search responses</returns>
        Task<Response> CreateAsync(SearchDetails searchDetails, List<SupplierResortSplit> resortSplits, IRequestTracker requestTracker);
    }
}