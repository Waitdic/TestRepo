namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class HotelReservationRequest
    {
        public HotelReservationRequest() { }

        [XmlAttribute("PrimaryLangID")]
        public string PrimaryLangID { get; set; } = string.Empty;

        [XmlAttribute("SequenceNmbr")]
        public string SequenceNmbr { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public List<HotelReservation> HotelReservations { get; set; } = new List<HotelReservation>();
    }
}