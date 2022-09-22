namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaReadRq : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("PrimaryLangID")]
        public string PrimaryLangID { get; set; } = string.Empty;

        [XmlArray("ReadRequests")]
        [XmlArrayItem("ReadRequest")]
        public List<ReadRequest> ReadRequests { get; set; } = new();
    }

    public class OtaResRetrieveRs : SoapContent
    {
        [XmlArray("ReservationsList")]
        [XmlArrayItem("HotelReservation")]
        public List<HotelReservation> ReservationsList { get; set; } = new();
    }
}
