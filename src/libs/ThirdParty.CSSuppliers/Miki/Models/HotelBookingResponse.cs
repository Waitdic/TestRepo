namespace ThirdParty.CSSuppliers.Miki.Models
{
    using System.Xml.Serialization;
    using Common;

    public class HotelBookingResponse : SoapContent
    {
        [XmlElement("booking")]
        public BookingBookResponse Booking { get; set; } = new();
    }
}
