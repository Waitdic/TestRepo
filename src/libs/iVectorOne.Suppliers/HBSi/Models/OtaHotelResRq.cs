namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelResRq : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("Target")]
        public string Target { get; set; } = string.Empty;

        [XmlAttribute("TimeStamp")]
        public string TimeStamp { get; set; } = string.Empty;

        [XmlAttribute("ResStatus")]
        public string ResStatus { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public List<HotelReservation> HotelReservations { get; set; } = new List<HotelReservation>();
    }
}
