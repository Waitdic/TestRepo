namespace iVectorOne.Search
{
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface that defines a class to process results
    /// </summary>
    public interface IExtraSearchResultsProcessor
    {
        // <summary>
        /// Processes the Third party results asynchronous.
        /// </summary>
        /// <param name="results">The results XML.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <returns>A count of the results</returns>
        Task<int> ProcessTPResultsAsync(TransformedTransferResultCollection results, ExtraSearchDetails searchDetails);

    }
}