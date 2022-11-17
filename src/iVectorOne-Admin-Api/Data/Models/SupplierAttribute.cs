namespace iVectorOne_Admin_Api.Data
{
    public partial class SupplierAttribute
    {
        public int SupplierAttributeId { get; set; }

        public short SupplierId { get; set; }

        public int AttributeId { get; set; }

        public virtual Config.Models.Attribute Attribute { get; set; } = null!;

        public virtual Supplier Supplier { get; set; } = null!;

        public virtual ICollection<AccountSupplierAttribute> AccountSupplierAttributes { get; set; } = new HashSet<AccountSupplierAttribute>();
    }
}