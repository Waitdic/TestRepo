namespace ThirdParty.Repositories
{
    using System.Threading.Tasks;

    /// <summary>Repository for retrieving third party meal basis</summary>
    public interface IMealBasisLookupRepository
    {
        /// <summary>Gets the meal basis from third party meal basis code.</summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party meal basis.</param>
        /// <returns>The matching meal basis</returns>
        Task<string> GetMealBasisfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis);

        /// <summary>Gets the meal basis ID from third party meal basis code.</summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party meal basis.</param>
        /// <returns>The matching meal basis ID</returns>
        Task<int> GetMealBasisIDfromTPMealbasisCodeAsync(string source, string thirdPartyMealBasis);

        /// <summary>Gets the meal basis code from third party meal basis id.</summary>
        /// <param name="source">The source.</param>
        /// <param name="thirdPartyMealBasis">The third party meal basis.</param>
        /// <returns>The matching meal basis code</returns>
        Task<string> GetMealBasisCodefromTPMealbasisIDAsync(string source, int thirdPartyMealBasis);
    }
}