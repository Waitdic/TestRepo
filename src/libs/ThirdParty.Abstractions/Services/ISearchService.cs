﻿namespace ThirdParty.Services
{
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using PropertySearch = SDK.V2.PropertySearch;

    /// <summary>
    /// Defines a service for performing searches
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Searches the specified search request.
        /// </summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="user">The user.</param>
        /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <param name="requestTracker"></param>
        /// <returns>A property search response</returns>
        Task<PropertySearch.Response> SearchAsync(PropertySearch.Request searchRequest, User user, bool log, IRequestTracker requestTracker);
    }
}