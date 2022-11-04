namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iVectorOne.Models;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    public interface ITransferSearchRepository
    {
        /// <summary>
        /// Gets the resort splits.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="suppliers">The suppliers.</param>
        /// <param name="account">The API Login</param>
        /// <returns>A list of resort splits</returns>
        Task<List<SupplierResortSplit>> GetResortSplitsAsync(string properties, string suppliers, Account account);
    }
}