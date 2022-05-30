namespace ThirdParty.CSSuppliers.NetStorming.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.NetStorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingBookResponse : EnvelopeBase
    {
        [XmlElement("response")]
        public BookResponse Response { get; set; } = new();

        public class BookResponse
        {
            [XmlElement(ElementName = "booking")]
            public Booking Booking { get; set; } = new();

            [XmlElement(ElementName = "status")]
            public Status Status { get; set; } = new();

            [XmlAttribute("type")]
            public string Type { get; set; } = string.Empty;
        }
    }
}