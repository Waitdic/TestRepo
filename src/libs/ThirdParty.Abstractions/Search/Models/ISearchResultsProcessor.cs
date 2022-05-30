namespace ThirdParty
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iVector.Search.Property;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    /// <summary>
    /// An interface that defines a class to process results
    /// </summary>
    public interface ISearchResultsProcessor
    {
        /// <summary>
        /// Processes the Third party results asynchronous.
        /// </summary>
        /// <param name="results">The results XML.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <returns>A count of the results</returns>
        Task<int> ProcessTPResultsAsync(TransformedResultCollection results, string source, SearchDetails searchDetails, IEnumerable<IResortSplit> resortSplits);
    }
}