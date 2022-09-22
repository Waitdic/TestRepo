namespace iVectorOne_Admin_Api.Data
{
    public partial class Supplier
    {
        public short SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;

        public virtual ICollection<SupplierAttribute> SupplierAttributes { get; set; } = new HashSet<SupplierAttribute>();
        public List<AccountSupplier> AccountSuppliers { get; set; } = new();
    }
}