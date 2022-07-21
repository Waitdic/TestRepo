using System.Xml.Serialization;

namespace iVectorOne.Suppliers.RMI.Models
{
    [XmlRoot("CancelResponse")]
    public class CancelResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();
        public BookingDetails BookingDetails { get; set; } = new();
    }
}
