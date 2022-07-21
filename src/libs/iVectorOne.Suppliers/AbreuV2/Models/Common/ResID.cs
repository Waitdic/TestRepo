namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class ResID
    {
        [XmlAttribute("Type")]
        public string IdType { get; set; } = string.Empty;

        [XmlAttribute("ID")]
        public string ID { get; set; } = string.Empty;

    }
}
