namespace iVectorOne_Admin_Api.Data
{
    public class Supplier
    {
        public short SupplierId { get; set; }

        public string SupplierName { get; set; } = null!;

        public string? TestPropertyIDs { get; set; } = null!;

        public virtual List<SupplierAttribute> SupplierAttributes { get; set; } = new List<SupplierAttribute>();

        public List<AccountSupplier> AccountSuppliers { get; set; } = new();
    }
}