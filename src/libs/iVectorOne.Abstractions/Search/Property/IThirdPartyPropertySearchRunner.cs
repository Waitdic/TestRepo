namespace iVectorOne.Search
{
    using System.Threading;
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;
    using iVectorOne.Search.Models;

    /// <summary>
    /// An interface representing a third party search runner class
    /// </summary>
    public interface IThirdPartyPropertySearchRunner
    {
        /// <summary>
        /// Runs the third party searches
        /// </summary>
        /// <param name="searchDetails">The search details</param>
        /// <param name="resortSplits">The resort splits</param>
        /// <param name="thirdPartySearch">The third party search adapter to run</param>
        /// <param name="cancellationTokenSource">The cancellation token source</param>
        /// <returns>The Task representing the search result</returns>
        Task SearchAsync(
            SearchDetails searchDetails,
            SupplierResortSplit resortSplits,
            IThirdPartySearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource);
    }
}