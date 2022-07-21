namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelResRq : SoapContent
    {
        public OtaHotelResRq() { }

        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("ResStatus")]
        public string ResStatus { get; set; } = string.Empty;

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public List<HotelReservation> HotelReservations { get; set; } = new List<HotelReservation>();
    }
}
