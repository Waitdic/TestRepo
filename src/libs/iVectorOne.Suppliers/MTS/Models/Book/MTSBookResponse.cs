namespace iVectorOne.Suppliers.MTS.Models.Book
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [Serializable()]
    [XmlRoot("OTA_HotelResRS")]
    public class MTSBookResponse
    {
        [XmlArray("HotelReservations")]
        [XmlArrayItem("HotelReservation")]
        public HotelReservation[] HotelReservations { get; set; } = Array.Empty<HotelReservation>();

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}
