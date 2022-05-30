using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelAvail")]
    public class HotelAvailability
    {
        public HotelAvailability()
        {
        }

        public HotelAvailability(HotelAvailabilityRequest hotelAvailabilityRequest, string xmlns)
        {
            HotelAvailabilityRequest = hotelAvailabilityRequest;
            Xmlns = xmlns;
        }

        [XmlElement(ElementName = "HotelAvailRQ")]
        public HotelAvailabilityRequest HotelAvailabilityRequest { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
