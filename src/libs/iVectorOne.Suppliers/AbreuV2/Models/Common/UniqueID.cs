namespace iVectorOne.Suppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class UniqueID
    {
        [XmlAttribute("Type")]
        public string IdType { get; set; } = string.Empty;
        [XmlAttribute("ID")]
        public string ID { get; set; } = string.Empty;
    }
}
