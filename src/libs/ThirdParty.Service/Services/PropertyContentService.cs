namespace ThirdParty.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.Repositories;
    using PropertyList = SDK.V2.PropertyList;
    using PropertyContent = SDK.V2.PropertyContent;

    /// <summary>Property content service responsible for retrieving third party content for properties.</summary>
    public class PropertyContentService : IPropertyContentService
    {
        /// <summary>Talks to database to retrieve property content</summary>
        private readonly IPropertyContentRepository _contentRepo;

        /// <summary>Initializes a new instance of the <see cref="PropertyContentService" /> class.</summary>
        /// <param name="contentRepo">The content repo.</param>
        public PropertyContentService(IPropertyContentRepository contentRepo)
        {
            _contentRepo = contentRepo;
        }

        /// <summary>Gets a list of all central property ids filtering by the past in last modified and suppliers</summary>
        /// <param name="lastModified">The last modified date is used to return only properties that have been imported after that date</param>
        /// <param name="suppliers">Filters the property ids to only those for the provided suppliers</param>
        /// <param name="user">The user making the request</param>
        /// <returns>A list of property ids</returns>
        public async Task<PropertyList.Response> PropertyListAsync(DateTime? lastModified, string suppliers, User user)
        {
            if (!lastModified.HasValue)
            {
                lastModified = new DateTime(2000, 01, 01);
            }

            suppliers = this.GetValidSuppliers(suppliers, user);

            var propertyIDs = await _contentRepo.GetPropertyIDsAsync(lastModified.Value, suppliers);

            var response = new PropertyList.Response()
            {
                Properties = propertyIDs
            };

            return response;
        }

        /// <summary>Returns the property content for each of the property ids passed in</summary>
        /// <param name="propertyIDs">The central property identifiers for the properties the content is being requested for</param>
        /// <param name="user">The user making the request</param>
        /// <returns>A list of property ids</returns>
        public async Task<PropertyContent.Response> PropertyContentAsync(List<int> propertyIDs, User user)
        {
            string suppliers = this.GetValidSuppliers(string.Empty, user);

            return await _contentRepo.GetPropertyContentAsync(propertyIDs, suppliers);
        }

        /// <summary>Validate suppliers, only returning suppliers that are configured</summary>
        /// <param name="suppliers">The suppliers.</param>
        /// <param name="user">The user.</param>
        /// <returns>a list of validated suppliers</returns>
        private string GetValidSuppliers(string suppliers, User user)
        {
            var validatedSuppliers = new List<string>();

            // 1. the configuration contains the suppliers the user has set up
            var configuredSuppliers = user.Configurations.Select(c => c.Supplier).ToList();

            // 2. Loop through the suppliers the user is trying to filter by, and ensure they are all suppliers that they have set up
            if (!string.IsNullOrEmpty(suppliers))
            {
                string[] supplierList = suppliers.Split(',');
                foreach (var supplier in supplierList)
                {
                    if (configuredSuppliers.Any(cs => cs.Equals(supplier.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase)))
                    {
                        validatedSuppliers.Add(supplier.Replace(" ", string.Empty));
                    }
                }
            }

            // 3. If they have not passed in any valid suppliers just use all
            // their configured suppliers (will be used if they dont pass any in)
            if (!validatedSuppliers.Any())
            {
                validatedSuppliers = configuredSuppliers;
            }

            // 4. Finally join them back into a string
            suppliers = string.Join(",", validatedSuppliers);
            return suppliers;
        }
    }
}