namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Document
    {
        [XmlAttribute("type")]
        public string? Type { get; set; }
    }
}
