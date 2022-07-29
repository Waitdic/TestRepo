using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelBookingRulesResponse")]
    public class HotelBookingRulesResponse
    {
        [XmlElement(ElementName = "BookingRulesRS")]
        public BookingRulesRS BookingRulesRS { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
