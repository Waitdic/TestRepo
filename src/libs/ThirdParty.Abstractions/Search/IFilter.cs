namespace ThirdParty.Search
{
    using System.Collections.Generic;
    using iVector.Search.Property;
    using ThirdParty.Search.Models;

    /// <summary>
    /// Filters property results
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Processes the results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>a list of property</returns>
        List<PropertySearchResult> ProcessResults(List<PropertySearchResult> results, IEnumerable<IResortSplit> resortSplits, SearchDetails searchDetails);
    }
}