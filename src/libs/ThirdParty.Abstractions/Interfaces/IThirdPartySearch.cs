﻿namespace ThirdParty
{
    using System.Collections.Generic;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    /// <summary>
    /// Defines a third party
    /// </summary>
    public interface IThirdPartySearch
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Builds the search requests.
        /// </summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <param name="saveLogs">if set to <c>true</c> [b save logs].</param>
        /// <returns>A list of request</returns>
        List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs);

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
        bool SearchRestrictions(SearchDetails searchDetails);

        /// <summary>
        /// Responses the has exceptions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>a boolean if the response has an exception</returns>
        bool ResponseHasExceptions(Request request);
    }
}