namespace iVectorOne.CSSuppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using iVectorOne.CSSuppliers.Hotelston.Models.Common;

    public class CancelHotelBookingRequest : RequestBase
    {
        [XmlElement("bookingReference")]
        public string BookingReference { get; set; } = string.Empty;
    }
}
