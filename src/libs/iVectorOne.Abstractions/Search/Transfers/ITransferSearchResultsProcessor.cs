namespace iVectorOne.Search
{
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    /// <summary>
    /// An interface that defines a class to process results
    /// </summary>
    public interface ITransferSearchResultsProcessor
    {
        // <summary>
        /// Processes the Third party results asynchronous.
        /// </summary>
        /// <param name="results">The results XML.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <returns>A count of the results</returns>
        int ProcessTPResultsAsync(TransformedTransferResultCollection results, string source, TransferSearchDetails searchDetails);

    }
}