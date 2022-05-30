using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Document")]
    public class Document
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
