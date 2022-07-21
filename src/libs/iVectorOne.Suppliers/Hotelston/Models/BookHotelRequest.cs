namespace ThirdParty.CSSuppliers.Hotelston.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    public class BookHotelRequest : RequestBase
    {
        [XmlElement("checkIn")]
        public string CheckIn { get; set; } = string.Empty;

        [XmlElement("checkOut")]
        public string CheckOut { get; set; } = string.Empty;

        [XmlElement("hotelId")]
        public string HotelId { get; set; } = string.Empty;

        [XmlElement("agentReferenceNumber")]
        public string AgentReferenceNumber { get; set; } = string.Empty;

        [XmlElement("confirmedBooking")]
        public string ConfirmedBooking { get; set; } = string.Empty;

        [XmlElement("contactPerson")]
        public ContactPerson ContactPerson { get; set; } = new();

        [XmlElement("room")]
        public List<BookRoom> Rooms { get; set; } = new();
    }
}
