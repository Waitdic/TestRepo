using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class Subscription
    {
        public Subscription()
        {
            SupplierSubscriptionAttributes = new HashSet<SupplierSubscriptionAttribute>();
            SupplierSubscriptions = new HashSet<SupplierSubscription>();
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
        public int? TenantId { get; set; }

        [JsonIgnore]
        public virtual Tenant? Tenant { get; set; }
        public virtual ICollection<SupplierSubscriptionAttribute> SupplierSubscriptionAttributes { get; set; }
        public virtual ICollection<SupplierSubscription> SupplierSubscriptions { get; set; }
    }
}
