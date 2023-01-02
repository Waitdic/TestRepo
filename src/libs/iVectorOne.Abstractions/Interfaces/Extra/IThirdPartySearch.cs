namespace iVectorOne.Extra
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
        Task<List<Request>> BuildSearchRequestsAsync(ExtraSearchDetails searchDetails, LocationMapping location);

        /// <summary>
        /// Transforms the response.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// /// <param name="searchDetails">The extra search details.</param>
        /// <param name="location">The location mapping.</param>
        /// <returns>an XML document</returns>
        TransformedExtraResultCollection TransformResponse(List<Request> requests, ExtraSearchDetails searchDetails, LocationMapping location);

        /// <summary>
        /// Check if there are any search restrictions for the third party.
        /// For example; the third party can not perform multi-room bookings.
        /// </summary>
        /// <param name="searchDetails">The extra search details.</param>
        /// <returns>If there any search restrictions for the third party.</returns>
        bool SearchRestrictions(ExtraSearchDetails searchDetails);

        /// <summary>
        /// Responses the has exceptions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>a boolean if the response has an exception</returns>
        bool ResponseHasExceptions(Request request);

        /// <summary>
        /// Validate the third party Settings.
        /// </summary>
        /// <param name="extraDetails">The extra search details.</param>
        /// <returns>boolean representing if the setting are present or not.</returns>
        bool ValidateSettings(ExtraSearchDetails searchDetails);
    }
}