namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelResRs : SoapContent
    {
        public OtaHotelResRs() { }

        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public List<HotelReservation> HotelReservations { get; set; } = new List<HotelReservation>();

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<ResponseError> Errors { get; set; } = new List<ResponseError>();

        [XmlElement("Success")]
        public string Success { get; set; } = string.Empty;
    }
}
