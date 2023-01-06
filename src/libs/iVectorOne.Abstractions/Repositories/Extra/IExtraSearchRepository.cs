namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    /// <summary>
    /// A repository for returning search information from the database
    /// </summary>
    public interface IExtraSearchRepository
    {
        /// <summary>
        /// Gets the resort splits.
        /// </summary>
        /// <param name="searchDetails">The extra search details.</param>
        /// <param name="account">The API Login</param>
        /// <returns>A list of resort splits</returns>
        //Task<LocationMapping> GetLocationMappingAsync(ExtraSearchDetails searchDetails);
        //Task AddLocations(string source, List<string> newLocations);
    }
}