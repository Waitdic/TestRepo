namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            SupplierAttributes = new HashSet<SupplierAttribute>();
            SupplierSubscriptions = new HashSet<SupplierSubscription>();
        }

        public short SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;

        public virtual ICollection<SupplierAttribute> SupplierAttributes { get; set; }
        public virtual ICollection<SupplierSubscription> SupplierSubscriptions { get; set; }
    }
}
