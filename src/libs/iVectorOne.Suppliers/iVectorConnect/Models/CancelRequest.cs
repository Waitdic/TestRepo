namespace iVectorOne.Suppliers.iVectorConnect.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("CancelRequest")]
    public class CancelRequest
    {
        public LoginDetails? LoginDetails { get; set; }

        public string BookingReference { get; set; } = string.Empty;

        public decimal CancellationCost { get; set; }

        public string CancellationToken { get; set; } = string.Empty;
    }
}