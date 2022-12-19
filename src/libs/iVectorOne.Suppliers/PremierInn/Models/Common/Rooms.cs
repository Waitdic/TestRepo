namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class Rooms
    {
        [XmlAttribute]
        public int NumberofRooms { get; set; }

        [XmlElement("RoomDetails")]
        public RoomDetails RoomDetails { get; set; } = new();
    }
}
