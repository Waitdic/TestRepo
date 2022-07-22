namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class SearchRequest
    {
        public SearchRequest() { }

        [XmlAttribute("type")]
        public string RequestType { get; set; } = Constant.RequestTypeSearch;

        [XmlAttribute("version")]
        public string Version { get; set; } = Constant.ApiVersion;

        [XmlAttribute("currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlElement("Session")]
        public Session Session { get; set; } = new();

        [XmlElement("Destination")]
        public Destination Destination { get; set; } = new();

        [XmlElement("Arrival")]
        public string Arrival { get; set; } = string.Empty;

        [XmlElement("Departure")]
        public string Departure { get; set; } = string.Empty;

        [XmlElement("NumberOfRooms")]
        public int NumberOfRooms { get; set; } = 1;

        [XmlArray("RoomsOccupancy")]
        [XmlArrayItem("Room")]
        public List<RequestRoom> RoomOccupancy { get; set; } = new();

        [XmlElement("Market")]
        public Market Market { get; set; } = new();
    }
}
