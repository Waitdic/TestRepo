using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.RMI.Models
{
    [XmlRoot("CancelResponse")]
    public class CancelResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();
        public BookingDetails BookingDetails { get; set; } = new();
    }
}
