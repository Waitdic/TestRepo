namespace iVectorOne_Admin_Api.Data
{
    public partial class SupplierSubscriptionAttribute
    {
        public int SupplierSubscriptionAttributeId { get; set; }
        public int SubscriptionId { get; set; }
        public int SupplierAttributeId { get; set; }
        public string Value { get; set; } = null!;

        public virtual Subscription Subscription { get; set; } = null!;
        public virtual SupplierAttribute SupplierAttribute { get; set; } = null!;
    }
}
