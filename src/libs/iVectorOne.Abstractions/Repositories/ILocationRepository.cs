namespace iVectorOne.Repositories
{
    using iVectorOne.SDK.V2.LocationContent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for returning locations from the database
    /// </summary>
    public interface ILocationRepository
    {
        /// <summary>
        /// Get all the locations which matches the supplier.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>List of Location</returns>
        Task<List<Location>> GetAllLocations(string source);
    }
}
