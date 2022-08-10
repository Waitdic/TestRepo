namespace iVectorOne_Admin_Api.Data
{
    public partial class SupplierSubscription
    {
        public int SupplierSubscriptionId { get; set; }
        public short SupplierId { get; set; }
        public int SubscriptionId { get; set; }

        public bool Enabled { get; set; }

        public Subscription Subscription { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
    }
}
