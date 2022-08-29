namespace iVectorOne.Suppliers.MTS.Models.Prebook
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [Serializable()]
    [XmlRoot("OTA_HotelResRS", IsNullable = false, Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSPrebookResponse
    {
        [XmlElement("HotelReservations")]
        public HotelReservation[] HotelReservations { get; set; } = Array.Empty<HotelReservation>();
    }
}
