namespace iVectorOne_Admin_Api.Data
{
    public partial class Supplier
    {
        public Supplier()
        {
            SupplierAttributes = new HashSet<SupplierAttribute>();
            SupplierSubscriptions = new List<SupplierSubscription>();
        }

        public short SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;

        public virtual ICollection<SupplierAttribute> SupplierAttributes { get; set; }
        public List<SupplierSubscription> SupplierSubscriptions { get; set; }
    }
}
