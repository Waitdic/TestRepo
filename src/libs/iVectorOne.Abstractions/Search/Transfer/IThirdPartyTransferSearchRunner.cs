namespace iVectorOne.Search
{
    using System.Threading;
    using System.Threading.Tasks;
    using iVectorOne.Transfer;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    /// <summary>
    /// An interface representing a third party search runner class
    /// </summary>
    public interface IThirdPartyTransferSearchRunner
    {
        /// <summary>
        /// Runs the third party searches
        /// </summary>
        /// <param name="searchDetails">The transfer search details</param>
        /// <param name="locationMapping">The transfer location mapping</param>
        /// <param name="thirdPartySearch">The third party search adapter to run</param>
        /// <param name="cancellationTokenSource">The cancellation token source</param>
        /// <returns>The Task representing the search result</returns>
        Task SearchAsync(
            TransferSearchDetails searchDetails,
            LocationMapping locationMapping,
            IThirdPartySearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource);
    }
}