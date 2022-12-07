namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class Attribute
    {
        public int AttributeId { get; set; }

        public string AttributeName { get; set; } = null!;

        public string? DefaultValue { get; set; }

        public virtual ICollection<SupplierAttribute> SupplierAttributes { get; set; } = new HashSet<SupplierAttribute>();

        public string Schema { get; set; } = string.Empty;

        public bool IsLegacyFormat { get; set; }

        public string FieldType { get; set; } = string.Empty;

        public string FieldDescription { get; set; } = string.Empty;

        public string FieldLabel { get; set; } = string.Empty;

        public int FieldOrder { get; set; }

        public int FieldMaxLength { get; set; }

        public int FieldMinLength { get; set; }

        public string FieldDefault { get; set; } = string.Empty;

        public bool FieldRequired { get; set; }
    }
}