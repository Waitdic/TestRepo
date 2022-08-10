using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Data
{
    public partial class SupplierAttribute
    {
        public SupplierAttribute()
        {
            SupplierSubscriptionAttributes = new HashSet<SupplierSubscriptionAttribute>();
        }

        public int SupplierAttributeId { get; set; }
        public short SupplierId { get; set; }
        public int AttributeId { get; set; }

        public virtual Config.Models.Attribute Attribute { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<SupplierSubscriptionAttribute> SupplierSubscriptionAttributes { get; set; }
    }
}
