using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelInfo")]
    public class HotelInfo
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Address")]
        public string Address { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "JPCode")]
        public string JPCode { get; set; }
        [XmlAttribute(AttributeName = "JPDCode")]
        public string JPDCode { get; set; }
        [XmlAttribute(AttributeName = "DestinationZone")]
        public string DestinationZone { get; set; }
    }
}
