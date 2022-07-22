using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Zone")]
    public class Zone
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "JPDCode")]
        public string JPDCode { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }
}
