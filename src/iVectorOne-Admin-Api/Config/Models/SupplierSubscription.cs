namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class SupplierSubscription
    {
        public int SupplierSubscriptionId { get; set; }
        public short SupplierId { get; set; }
        public int SubscriptionId { get; set; }

        public virtual Subscription Subscription { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
    }
}
