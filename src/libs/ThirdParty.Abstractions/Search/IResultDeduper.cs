namespace ThirdParty.Search
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdParty.Search.Models;

    /// <summary>
    /// An object that de-duplicates third party property results
    /// </summary>
    public interface IResultDeduper
    {
        /// <summary>
        /// de-duplicates the results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>a count of results</returns>
        Task<int> DedupeResultsAsync(List<iVector.Search.Property.PropertySearchResult> results, SearchDetails searchDetails);
    }
}