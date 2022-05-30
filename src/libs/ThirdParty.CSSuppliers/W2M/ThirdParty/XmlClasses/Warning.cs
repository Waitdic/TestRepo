using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Warning")]
    public class Warning
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "Text")]
        public string Text { get; set; }
    }
}
