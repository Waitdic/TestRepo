namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class AccommodationRequest
    {
        [XmlAttribute("code")]
        public string? Code { get; set; }

        [XmlAttribute("concept")]
        public string? Concept { get; set; }

        [XmlAttribute("board")]
        public string? Board { get; set; }

        [XmlAttribute("offer")]
        public string? Offer { get; set; }

        [XmlAttribute("ticket")]
        public string? Ticket { get; set; }
    }
}
