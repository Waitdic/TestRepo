namespace iVectorOne.CSSuppliers.Hotelston.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    public class BookingTermsRequest : RequestBase
    {
        [XmlElement("hotelId")]
        public string HotelId { get; set; } = string.Empty;

        [XmlElement("searchId")]
        public string SearchId { get; set; } = string.Empty;

        [XmlElement("room")]
        public List<Room> Rooms { get; set; } = new();
    }
}
