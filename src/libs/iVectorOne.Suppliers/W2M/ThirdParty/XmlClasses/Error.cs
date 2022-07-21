using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Error")]
    public class Error
    {
        [XmlAttribute(AttributeName = "Text")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }
}
