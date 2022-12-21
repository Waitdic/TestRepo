namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// produced transfer search responses
    /// </summary>
    public interface IExtraSearchResponseFactory
    {
        /// <summary>Creates a search response object</summary>
        /// <param name="searchDetails">The search details, which contains the results used for building the response</param>
        /// <param name="locationMapping">The transfer location mapping containing the third party location data</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of response times</param>
        /// <returns>A property search responses</returns>
        Task<Response> CreateAsync(ExtraSearchDetails searchDetails, LocationMapping locationMapping, IRequestTracker requestTracker);
    }
}