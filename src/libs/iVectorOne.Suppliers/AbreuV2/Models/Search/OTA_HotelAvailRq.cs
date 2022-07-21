namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OTA_HotelAvailRQ : SoapContent
    {
        public HotelSearch HotelSearch { get; set; } = new();
    }

    public class HotelSearch
    {
        public Currency Currency { get; set; } = new();
        public HotelRef HotelRef { get; set; } = new();
        public DateRange DateRange { get; set; } = new();
        [XmlArray("RoomCandidates")]
        [XmlArrayItem("RoomCandidate")]
        public List<RoomCandidate> RoomCandidates { get; set; } = new();
    }
}
