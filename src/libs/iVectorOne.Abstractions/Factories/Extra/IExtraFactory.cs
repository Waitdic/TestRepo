namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a factory that takes in a third party extra search request and returns a list of extras
    /// </summary>
    public interface IExtraFactory
    {
        /// <summary>Creates extras.</summary>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="account">The account</param>
        /// <returns>A list of extras</returns>
        Task<List<Extras>> CreateAsync(ExtraSearchDetails searchDetails, Account account);
    }
}