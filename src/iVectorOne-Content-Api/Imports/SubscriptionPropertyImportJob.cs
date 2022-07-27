namespace iVectorOne.Content.Imports
{
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Scheduling;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using TPImport.Local.Properties;

    [ScheduledJob("Subscription Property Import", trigger: TriggerType.Once)]
    public class SubscriptionPropertyImportJob : IScheduledJob
    {
        private readonly ISql _sql;
        private readonly IPropertyListService _propertyListService;
        private readonly ILogger<SubscriptionPropertyImportJob> _logger;

        public SubscriptionPropertyImportJob(ISql sql, IPropertyListService propertyListService, ILogger<SubscriptionPropertyImportJob> logger)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _propertyListService = Ensure.IsNotNull(propertyListService, nameof(propertyListService));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task ExecuteAsync(IScheduledJobContext context)
        {
            try
            {
                var customers = await _sql.ReadAllAsync<Customer>(
                    "Customer_GetSingleTenantCustomers",
                    new CommandSettings().IsStoredProcedure());

                foreach (var customer in customers)
                {
                    await ProcessCustomerProperties(customer);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{message}", e.Message);
            }
        }

        private async Task ProcessCustomerProperties(Customer customer)
        {
            try
            {
                var request = new GetOwnStockRequest()
                {
                    BaseAddress = customer.BaseUrl,
                    DateModifiedSince = DateTime.Now.AddDays(-1),
                    Sources = customer.Suppliers,
                };

                var properties = await _propertyListService.GetOwnStockPropertiesAsync(request);

                foreach (var property in properties.Where(p => customer.SuppliersList.Contains(p.Source)))
                {
                    await ImportProperty(customer, property);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{message}", e.Message);
            }
        }

        private async Task ImportProperty(Customer customer, Intuitive.TPImport.Models.PropertyDetails property)
        {
            try
            {
                bool newProperty = !customer.PropertyIDSet.Contains(property.TPKey);

                var settings = new CommandSettings()
                    .IsStoredProcedure()
                    .WithParameters(new
                    {
                        @subscriptionId = customer.SubscriptionID,
                        @source = property.Source,
                        @tpKey = property.TPKey,
                        @name = property.Name,
                        @propertyDetails = JsonConvert.SerializeObject(property),
                        @checksum = property.Checksum,
                        @ttiCode = property.TTICode.ToSafeString(),
                        @descriptionLength = property.Description?.Length ?? 0,
                        @hasPostcode = !string.IsNullOrEmpty(property.Postcode),
                        @hasPhoneNumber = !string.IsNullOrEmpty(property.Telephone),
                        @rating = property.Rating ?? "0.0",
                    });

                if (newProperty)
                {
                    await _sql.ExecuteAsync("Property_AddSubscriptionProperty", settings);
                }
                else
                {
                    await _sql.ExecuteAsync("Property_UpdateSubscriptionProperty", settings);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{message}", e.Message);
            }
        }

        internal class Customer
        {
            public int CustomerID { get; set; }
            public string Name { get; set; } = string.Empty;
            public string BaseUrl { get; set; } = string.Empty;
            public int SubscriptionID { get; set; }
            public string Suppliers { get; set; } = string.Empty;
            public string PropertyIDs { get; set; } = string.Empty;
            public List<string> SuppliersList
                => Suppliers.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
            public HashSet<string> PropertyIDSet
                => PropertyIDs.Split(",", StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        }
    }
}