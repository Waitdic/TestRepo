namespace iVectorOne
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    /// <summary>
    /// Defines a third party transfer 
    /// </summary>
    public interface IThirdPartyTransferSearch
    {
        /// <summary>
        /// Builds the search requests.
        /// </summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <returns>A list of request</returns>
        Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, List<ResortSplit> resortSplits);

        /// <summary>
        /// Transforms the response.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>an XML document</returns>
        TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, List<ResortSplit> resortSplits);

        /// <summary>
        /// Check if there are any search restrictions for the third party.
        /// For example; the third party can not perform multi-room bookings.
        /// </summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <returns>If there any search restrictions for the third party.</returns>
        bool SearchRestrictions(TransferSearchDetails searchDetails, string source);

        /// <summary>
        /// Responses the has exceptions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>a boolean if the response has an exception</returns>
        bool ResponseHasExceptions(Request request);
    }
}