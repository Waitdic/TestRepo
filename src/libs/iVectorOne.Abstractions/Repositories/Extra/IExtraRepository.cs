namespace iVectorOne.Repositories
{
    using iVectorOne.SDK.V2.ExtraContent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for returning extras from the database
    /// </summary>
    public interface IExtraRepository
    {
        /// <summary>
        /// Get all the extras which matches the supplier.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>List of extra</returns>
        Task<List<Extra>> GetAllExtras(string source);
    }
}
