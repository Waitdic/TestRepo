namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class Attribute
    {
        public Attribute()
        {
            SupplierAttributes = new HashSet<SupplierAttribute>();
        }

        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string? DefaultValue { get; set; }

        public virtual ICollection<SupplierAttribute> SupplierAttributes { get; set; }
    }
}
