namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// produced extra search responses
    /// </summary>
    public interface IExtraSearchResponseFactory
    {
        /// <summary>Creates a search response object</summary>
        /// <param name="searchDetails">The search details, which contains the results used for building the response</param>
        /// <param name="extras">The extras containing the third party extra names</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of response times</param>
        /// <returns>An extra search response</returns>
        Task<Response> CreateAsync(ExtraSearchDetails searchDetails, List<Extra> extras, IRequestTracker requestTracker);
    }
}