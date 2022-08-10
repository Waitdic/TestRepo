namespace iVectorOne_Admin_Api.Data
{
    public partial class Subscription
    {
        public Subscription()
        {
            SupplierSubscriptionAttributes = new List<SupplierSubscriptionAttribute>();
            SupplierSubscriptions = new List<SupplierSubscription>();
        }

        public int SubscriptionId { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? DummyResponses { get; set; }
        public short PropertyTprequestLimit { get; set; }
        public short SearchTimeoutSeconds { get; set; }
        public bool LogMainSearchError { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public string Environment { get; set; } = null!;
        public int TenantId { get; set; }
        public string Status { get; set; } = null!;

        public Tenant? Tenant { get; set; }
        public List<SupplierSubscriptionAttribute> SupplierSubscriptionAttributes { get; set; }
        public List<SupplierSubscription> SupplierSubscriptions { get; set; }
    }
}
