namespace ThirdParty.CSSuppliers.ATI.Models
{
    using System;
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.ATI.Models.Common;

    public class AtiBookRequest : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("TransactionIdentifier")]
        public int TransactionIdentifier { get; set; }

        [XmlElement("POS")]
        public Pos POS { get; set; } = new();

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public HotelReservation[] HotelReservations { get; set; } = Array.Empty<HotelReservation>();
    }
}
