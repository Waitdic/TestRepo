namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class CancellationResult
    {
        public CancellationResult() { }

        [XmlElement("status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("Currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlElement("CancellationPrice")]
        public string CancellationPrice { get; set; } = string.Empty;

        [XmlElement("BookingId")]
        public string BookingId { get; set; } = string.Empty;
    }
}
