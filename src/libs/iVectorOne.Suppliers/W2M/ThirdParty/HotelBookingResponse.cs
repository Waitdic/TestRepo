using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelBookingResponse")]
    public class HotelBookingResponse
    {
        [XmlElement(ElementName = "BookingRS")]
        public BookingRS BookingRS { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
