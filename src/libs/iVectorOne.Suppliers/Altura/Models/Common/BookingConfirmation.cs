namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class BookingConfirmation
    {
        public BookingConfirmation() { }

        [XmlAttribute("status")]
        public string Status { get; set; } = string.Empty;

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("RateId")]
        public string RateId { get; set; } = string.Empty;

        [XmlElement("SpecialRequest")]
        public string SpecialRequest { get; set; } = string.Empty;
    }
}
