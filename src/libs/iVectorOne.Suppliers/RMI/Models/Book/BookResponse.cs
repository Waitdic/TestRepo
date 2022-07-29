namespace iVectorOne.Suppliers.RMI.Models
{
    using System.Xml.Serialization;

    [XmlRoot("BookResponse")]
    public class BookResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();
        public BookingDetails BookingDetails { get; set; } = new();
    }
}
