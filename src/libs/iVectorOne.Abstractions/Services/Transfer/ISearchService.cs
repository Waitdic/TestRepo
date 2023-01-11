namespace iVectorOne.Services.Transfer
{
    using System.Threading.Tasks;
    using iVectorOne.Search.Models;
    using TransferSearch = SDK.V2.TransferSearch;

    /// <summary>
    /// Defines a service for performing searches
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Searches the specified search request.
        /// </summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <param name="requestTracker"></param>
        /// <returns>A transfer search response</returns>
        Task<TransferSearch.Response> SearchAsync(TransferSearch.Request searchRequest, bool log, IRequestTracker requestTracker);
    }
}