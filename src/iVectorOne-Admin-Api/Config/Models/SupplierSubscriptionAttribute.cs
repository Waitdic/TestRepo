using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class SupplierSubscriptionAttribute
    {
        public int SupplierSubscriptionAttributeId { get; set; }
        public int SubscriptionId { get; set; }
        public int SupplierAttributeId { get; set; }
        public string Value { get; set; } = null!;

        [JsonIgnore]
        public virtual Subscription Subscription { get; set; } = null!;

        [JsonIgnore]
        public virtual SupplierAttribute SupplierAttribute { get; set; } = null!;
    }
}
