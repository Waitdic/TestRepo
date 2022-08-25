namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Models;
    using iVectorOne.Repositories;
    using PropertyContent = SDK.V2.PropertyContent;
    using PropertyList = SDK.V2.PropertyList;

    /// <summary>Property content service responsible for retrieving third party content for properties.</summary>
    public class PropertyContentService : IPropertyContentService
    {
        /// <summary>Talks to database to retrieve property content</summary>
        private readonly IPropertyContentRepository _contentRepo;

        /// <summary>Initializes a new instance of the <see cref="PropertyContentService" /> class.</summary>
        /// <param name="contentRepo">The content repo.</param>
        public PropertyContentService(IPropertyContentRepository contentRepo)
        {
            _contentRepo = Ensure.IsNotNull(contentRepo, nameof(contentRepo));
        }

        /// <inheritdoc/>
        public async Task<PropertyList.Response> PropertyListAsync(DateTime? lastModified, string suppliers, Account account)
        {
            if (!lastModified.HasValue)
            {
                lastModified = new DateTime(2000, 01, 01);
            }

            suppliers = this.GetValidSuppliers(suppliers, account);

            var propertyIDs = await _contentRepo.GetPropertyIDsAsync(lastModified.Value, suppliers, account);

            var response = new PropertyList.Response()
            {
                Properties = propertyIDs
            };

            return response;
        }

        /// <inheritdoc/>
        public async Task<PropertyContent.Response> PropertyContentAsync(List<int> propertyIDs, Account account)
        {
            string suppliers = this.GetValidSuppliers(string.Empty, account);

            return await _contentRepo.GetPropertyContentAsync(propertyIDs, suppliers, account);
        }

        /// <summary>Validate suppliers, only returning suppliers that are configured</summary>
        /// <param name="suppliers">The suppliers.</param>
        /// <param name="account">The account.</param>
        /// <returns>a list of validated suppliers</returns>
        private string GetValidSuppliers(string suppliers, Account account)
        {
            var validatedSuppliers = new List<string>();

            // 1. the configuration contains the suppliers the account has set up
            var configuredSuppliers = account.Configurations.Select(c => c.Supplier).ToList();

            // 2. Loop through the suppliers the account is trying to filter by, and ensure they are all suppliers that they have set up
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