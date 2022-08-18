namespace iVectorOne.Suppliers.Serhs.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("response")]
    public class SerhsCancellationResponse
    {
        [XmlElement("status")]
        public string? Status { get; set; }

        [XmlElement("booking")]
        public Booking Booking { get; set; } = new();

        [XmlElement("cancel")]
        public Cancel Cancel { get; set; } = new();

        [XmlElement("amount_details")]
        public AmountDetails AmountDetails { get; set; } = new();
    }
}
