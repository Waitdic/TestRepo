namespace iVectorOne.Search
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using iVectorOne.Extra;
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using iVectorOne.Search.Models;

    /// <summary>
    /// An interface representing a third party search runner class
    /// </summary>
    public interface IThirdPartyExtraSearchRunner
    {
        /// <summary>
        /// Runs the third party searches
        /// </summary>
        /// <param name="searchDetails">The extra search details</param>
        /// <param name="extras">The extras from database</param>
        /// <param name="thirdPartySearch">The third party search adapter to run</param>
        /// <param name="cancellationTokenSource">The cancellation token source</param>
        /// <returns>The Task representing the search result</returns>
        Task SearchAsync(
            ExtraSearchDetails searchDetails,
            List<Extras> extras,
            IThirdPartySearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource);
    }
}