namespace iVectorOne.Factories
{
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.PropertySearch;
    using iVectorOne.Search.Models;

    /// <summary>
    /// Defines a factory that takes in a third party search request and creations an abstraction search details
    /// </summary>
    public interface ISearchDetailsFactory
    {
        /// <summary>Creates the specified search request.</summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="account">The account</param>
        /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A search details</returns>
        SearchDetails Create(Request searchRequest, Account account, bool log);
    }
}