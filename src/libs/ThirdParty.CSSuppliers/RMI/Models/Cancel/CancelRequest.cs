using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.RMI.Models
{
    [XmlRoot("CancelRequest")]
    public class CancelRequest
    {
        public LoginDetails LoginDetails { get; set; } = new();
        public string BookingReference { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
