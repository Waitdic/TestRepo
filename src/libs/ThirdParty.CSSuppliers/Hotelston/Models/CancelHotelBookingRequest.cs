namespace ThirdParty.CSSuppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.Hotelston.Models.Common;

    public class CancelHotelBookingRequest : RequestBase
    {
        [XmlElement("bookingReference")]
        public string BookingReference { get; set; } = string.Empty;
    }
}
