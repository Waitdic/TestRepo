namespace iVectorOne.Suppliers.ATI.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class AtiBookResponse : SoapContent
    {
        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public HotelReservation[] HotelReservations { get; set; } = Array.Empty<HotelReservation>();

        public OTA_PkgBookResponse OTA_PkgBookRS { get; set; } = new();
    }
}
