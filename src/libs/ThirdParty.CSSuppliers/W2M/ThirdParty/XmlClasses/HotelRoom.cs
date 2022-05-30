using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelRoom")]
    public class HotelRoom
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "RoomCategory")]
        public RoomCategory RoomCategory { get; set; }
        [XmlElement(ElementName = "RoomOccupancy")]
        public RoomOccupancy RoomOccupancy { get; set; }
        [XmlAttribute(AttributeName = "Units")]
        public string Units { get; set; }
        [XmlAttribute(AttributeName = "Source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "AvailRooms")]
        public int AvailRooms { get; set; }
        [XmlElement(ElementName = "RelPaxes")]
        public RelativePaxes relativePaxes { get; set; }
        [XmlAttribute(AttributeName = "JRCode")]
        public string JRCode { get; set; }
    }
}
