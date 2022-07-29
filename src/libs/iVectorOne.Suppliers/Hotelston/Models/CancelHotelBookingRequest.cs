namespace iVectorOne.Suppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Hotelston.Models.Common;

    public class CancelHotelBookingRequest : RequestBase
    {
        [XmlElement("bookingReference")]
        public string BookingReference { get; set; } = string.Empty;
    }
}
