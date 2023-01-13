namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelResRs : SoapContent
    {
        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public List<HotelReservation> HotelReservations { get; set; } = new List<HotelReservation>();
    }
}
