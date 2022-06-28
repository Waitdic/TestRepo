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

        public string Schema { get; set; } = String.Empty;
    }
    public class Configuration
    {
        public string? key { get; set; }
        public int? order { get; set; }
        public bool? required { get; set; }
        public string? type { get; set; }
        public string? description { get; set; }
        public int? minimum { get; set; }
        public int? maximum { get; set; }
        public int? minLength { get; set; }
        public int? maxLength { get; set; }
        public List<Dropdownoption> dropdownOptions { get; set; } = new List<Dropdownoption>();
    }

    public class Dropdownoption
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
