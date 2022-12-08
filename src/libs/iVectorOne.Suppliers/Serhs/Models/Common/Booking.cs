namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Booking
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("confirmed")]
        public string? Confirmed { get; set; }

        [XmlAttribute("clientReference")]
        public string? ClientReference { get; set; }

        [XmlAttribute("cancelled")]
        public string? Cancelled { get; set; }

        [XmlAttribute("type")]
        public string? Type { get; set; }
    }
}