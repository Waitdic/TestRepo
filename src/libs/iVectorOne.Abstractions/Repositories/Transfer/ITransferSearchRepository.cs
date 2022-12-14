namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    public interface ITransferSearchRepository
    {
        /// <summary>
        /// Gets the resort splits.
        /// </summary>
        /// <param name="searchDetails">The transfer search details.</param>
        /// <param name="account">The API Login</param>
        /// <returns>A list of resort splits</returns>
        Task<LocationMapping> GetLocationMappingAsync(TransferSearchDetails searchDetails);
        Task AddLocations(string source, List<string> newLocations);
    }
}