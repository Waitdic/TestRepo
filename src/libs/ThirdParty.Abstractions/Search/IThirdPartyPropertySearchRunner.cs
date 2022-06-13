namespace ThirdParty.Search
{
    using System.Threading;
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    public interface IThirdPartyPropertySearchRunner
    {
        Task SearchAsync(
            SearchDetails searchDetails,
            SupplierResortSplit resortSplits,
            IThirdPartySearch thirdPartySearch,
            CancellationTokenSource cancellationTokenSource);
    }
}