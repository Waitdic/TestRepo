namespace iVectorOne.CSSuppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Board
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("ticket")]
        public string? Ticket { get; set; }

        [XmlAttribute("cancelPolicyId")]
        public string? CancelPolicyId { get; set; }

        [XmlElement("price")]
        public Price Price { get; set; } = new();

        [XmlElement("offer")]
        public List<Offer> Offers { get; set; } = new();
    }
}