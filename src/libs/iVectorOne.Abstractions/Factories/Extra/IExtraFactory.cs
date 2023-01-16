namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a factory that takes in a third party extra search request and creations a location map
    /// </summary>
    public interface IExtraFactory
    {
        /// <summary>Creates the extra location mapping.</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="account">The account</param>
        /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A extra location map</returns>
        Task<List<string>> CreateAsync(ExtraSearchDetails searchDetails, Account account);
    }
}