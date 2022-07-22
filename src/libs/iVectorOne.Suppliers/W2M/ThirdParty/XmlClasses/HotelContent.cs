using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelContent")]
    public class HotelContent
    {
        [XmlElement(ElementName = "HotelName")]
        public string HotelName { get; set; }
        [XmlElement(ElementName = "Zone")]
        public Zone Zone { get; set; }
        [XmlElement(ElementName = "HotelCategory")]
        public HotelCategory HotelCategory { get; set; }
        [XmlElement(ElementName = "Address")]
        public AddressType Address { get; set; }
        [XmlElement(ElementName = "PointsOfInterest")]
        public PointsOfInterest PointsOfInterest { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }
}
