namespace iVectorOne.Suppliers.MTS.Models.Prebook
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [Serializable()]
    [XmlRoot("OTA_HotelResRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSPrebookRequest
    {
        [XmlAttribute]
        public string EchoToken { get; set; } = string.Empty;

        [XmlAttribute]
        public string ResStatus { get; set; } = string.Empty;

        [XmlAttribute]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("schemaLocation")]
        public string SchemaLocation { get; set; } = string.Empty;

        [XmlElement]
        public POS POS { get; set; } = new();

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public HotelReservation[] HotelReservations { get; set; } = Array.Empty<HotelReservation>();
    }
}
