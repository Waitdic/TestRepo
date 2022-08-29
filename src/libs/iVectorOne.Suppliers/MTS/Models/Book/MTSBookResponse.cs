namespace iVectorOne.Suppliers.MTS.Models.Book
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [Serializable()]
    [XmlRoot("OTA_HotelResRS", IsNullable = false, Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSBookResponse
    {
        [XmlElement("HotelReservations")]
        public HotelReservation[] HotelReservations { get; set; } = Array.Empty<HotelReservation>();

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}
