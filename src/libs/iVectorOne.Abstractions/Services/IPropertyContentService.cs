namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using PropertyList = SDK.V2.PropertyList;
    using PropertyContent = SDK.V2.PropertyContent;

    /// <summary>Property content service responsible for retrieving third party content for properties.</summary>
    public interface IPropertyContentService
    {
        /// <summary>Returns the property content for each of the property ids passed in</summary>
        /// <param name="propertyIDs">Only provide content for the provided properties</param>
        /// <param name="user">The user making the request</param>
        /// <returns>the property content for all provided property ids</returns>
        Task<PropertyContent.Response> PropertyContentAsync(List<int> propertyIDs, Subscription user);

        /// <summary>Gets a list of all central property ids filtering by the past in last modified and suppliers</summary>
        /// <param name="lastModified">The last modified date is used to return only properties that have been imported after that date</param>
        /// <param name="suppliers">Filters the property ids to only those for the provided suppliers</param>
        /// <param name="user">The user making the request</param>
        /// <returns>A list of property ids</returns>
        Task<PropertyList.Response> PropertyListAsync(DateTime? lastModified, string suppliers, Subscription user);
    }
}