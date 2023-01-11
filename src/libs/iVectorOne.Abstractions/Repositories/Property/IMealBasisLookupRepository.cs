namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;

    // todo - remove duplicate code between here and tp support
    /// <summary>Repository for retrieving third party meal basis</summary>
    public interface IMealBasisLookupRepository
    {
        /// <summary>Gets the meal basis from third party meal basis code.</summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party meal basis.</param>
        /// <param name="accountId">The account identifier</param>
        /// <returns>The matching meal basis</returns>
        Task<string> GetMealBasisfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis, int accountId);

        /// <summary>Gets the meal basis ID from third party meal basis code.</summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party meal basis.</param>
        /// <param name="accountId">The account identifier</param>
        /// <returns>The matching meal basis ID</returns>
        Task<int> GetMealBasisIDfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis, int accountId);

        /// <summary>Gets the meal basis code from third party meal basis id.</summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party meal basis.</param>
        /// <param name="accountId">The account identifier</param>
        /// <returns>The matching meal basis code</returns>
        Task<string> GetMealBasisCodefromTPMealbasisIDAsync(string source, int thirdPartyMealBasis, int accountId);
    }
}