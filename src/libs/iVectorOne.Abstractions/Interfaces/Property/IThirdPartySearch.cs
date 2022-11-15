namespace iVectorOne
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    /// <summary>
    /// Defines a third party
    /// </summary>
    public interface IThirdPartySearch
    {
        /// <summary>
        /// Builds the search requests.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <returns>A list of request</returns>
        Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits);

        /// <summary>
        /// Transforms the response.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>an XML document</returns>
        TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits);

        /// <summary>
        /// Check if there are any search restrictions for the third party.
        /// For example; the third party can not perform multi-room bookings.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>If there any search restrictions for the third party.</returns>
        bool SearchRestrictions(SearchDetails searchDetails, string source);

        /// <summary>
        /// Responses the has exceptions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>a boolean if the response has an exception</returns>
        bool ResponseHasExceptions(Request request);
    }
}