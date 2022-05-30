using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelBooking")]
    public class HotelBooking
    {
        public HotelBooking(BookingRequest bookingRequest)
        {
            BookingRequest = bookingRequest;
        }

        public HotelBooking()
        {
        }

        [XmlElement(ElementName = "HotelBookingRQ")]
        public BookingRequest BookingRequest { get; set; }
    }
}
