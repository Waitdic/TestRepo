namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BasketBookResponse")]
    public class BasketBookResponse
    {
        [XmlElement("PropertyBookings")]
        public PropertyBookingsResponse PropertyBookings { get; set; } = new();

        [XmlElement("BookingReference")]
        public string BookingReference { get; set; } = string.Empty;
    }
}
