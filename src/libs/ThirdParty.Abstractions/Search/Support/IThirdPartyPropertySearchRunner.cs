namespace ThirdParty
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    public interface IThirdPartyPropertySearchRunner
    {
        Task SearchAsync(
            SearchDetails searchDetails,
            List<ResortSplit> resortSplits,
            IThirdPartySearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource);
    }
}