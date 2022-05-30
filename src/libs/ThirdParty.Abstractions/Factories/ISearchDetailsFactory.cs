namespace ThirdParty.Factories
{
    using ThirdParty.Models;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Search.Models;

    /// <summary>
    /// Defines a factory that takes in a third party search request and creations an abstraction search details
    /// </summary>
    public interface ISearchDetailsFactory
    {
        /// <summary>Creates the specified search request.</summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="user">The user</param>
        /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A search details</returns>
        SearchDetails Create(Request searchRequest, User user, bool log);
    }
}