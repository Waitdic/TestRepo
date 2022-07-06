namespace ThirdParty.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.SDK.V2.PropertyContent;

    /// <summary>Repository that retrieves property content from the database</summary>
    public interface IPropertyContentRepository
    {
        /// <summary>Gets a list of all central property ids filtering by the past in last modified and suppliers</summary>
        /// <param name="lastModified">The last modified date is used to return only properties that have been imported after that date</param>
        /// <param name="suppliers">Filters the property ids to only those for the provided suppliers</param>
        /// <param name="user">The api login</param>
        /// <returns>A list of property ids</returns>
        Task<List<int>> GetPropertyIDsAsync(DateTime lastModified, string suppliers, Subscription user);

        /// <summary>Returns the property content for each of the property ids passed in</summary>
        /// <param name="propertyIDs">The central property identifiers for the properties the content is being requested for</param>
        /// <param name="suppliers">Filters the property ids to only those for the provided suppliers</param>
        /// <param name="user">The api login</param>
        /// <returns>A property content response.</returns>
        Task<Response> GetPropertyContentAsync(List<int> propertyIDs, string suppliers, Subscription user);

        /// <summary>Takes in a property identifier and looks up the content for that property.</summary>
        /// <param name="propertyId">The property identifier (this will be the property identifier not the central property identifier that the other calls use).</param>
        /// <param name="user">The api login</param>
        /// <returns>a property content object</returns>
        Task<PropertyContent> GetContentforPropertyAsync(int propertyId, Subscription user);
    }
}