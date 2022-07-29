namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class Cancellation
    {
        public Cancellation() { }

        [XmlElement("BookingId")]
        public string BookingId { get; set; } = string.Empty;

        [XmlElement("CancellationPrice")]
        public decimal CancellationPrice { get; set; }

    }
}
