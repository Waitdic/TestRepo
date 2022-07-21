namespace ThirdParty.CSSuppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BookHotelResponse")]
    public class BookHotelResponse : SoapContent
    {
        [XmlElement("success")]
        public bool Success { get; set; }

        [XmlElement("bookingReference")]
        public string BookingReference { get; set; } = string.Empty;
    }
}
