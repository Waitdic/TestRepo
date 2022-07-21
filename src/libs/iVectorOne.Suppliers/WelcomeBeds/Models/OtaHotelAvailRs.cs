namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelAvailRs : SoapContent
    {
        public OtaHotelAvailRs() { }

        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlElement("Success")]
        public string Success { get; set; } = string.Empty;

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<ResponseError> Errors { get; set; } = new List<ResponseError>();

        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public List<RoomStay> RoomStays { get; set; } = new List<RoomStay>();

        [XmlIgnore]
        [XmlElement("POS")]
        public string Pos { get; set; } = string.Empty;

        [XmlElement("TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new TpaExtensions();


    }
}
