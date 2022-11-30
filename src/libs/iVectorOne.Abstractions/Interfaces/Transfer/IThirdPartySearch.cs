namespace iVectorOne.Transfer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    /// <summary>
    /// Defines a third party transfer 
    /// </summary>
    public interface IThirdPartySearch
    {
        /// <summary>
        /// Builds the search requests.
        /// </summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="location">The location mapping.</param>
        /// <returns>A list of request</returns>
        Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location);

        /// <summary>
        /// Transforms the response.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="location">The location mapping.</param>
        /// <returns>an XML document</returns>
        TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, LocationMapping location);

        /// <summary>
        /// Check if there are any search restrictions for the third party.
        /// For example; the third party can not perform multi-room bookings.
        /// </summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <returns>If there any search restrictions for the third party.</returns>
        bool SearchRestrictions(TransferSearchDetails searchDetails);

        /// <summary>
        /// Responses the has exceptions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>a boolean if the response has an exception</returns>
        bool ResponseHasExceptions(Request request);
    }
}