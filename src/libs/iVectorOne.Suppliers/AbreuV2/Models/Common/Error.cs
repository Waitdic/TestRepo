namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class Error
    {
        [XmlAttribute("Status")]
        public string Status { get; set; } = string.Empty;
    }
}
