using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelCheckAvailResponse")]
    public class CheckAvailabilityResponse
    {
        [XmlElement(ElementName = "CheckAvailRS")]
        public CheckAvailRS CheckAvailRS { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
