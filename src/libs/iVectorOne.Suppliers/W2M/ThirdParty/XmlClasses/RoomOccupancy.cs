using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "RoomOccupancy")]
    public class RoomOccupancy
    {
        [XmlAttribute(AttributeName = "Occupancy")]
        public string Occupancy { get; set; }
        [XmlAttribute(AttributeName = "Adults")]
        public string Adults { get; set; }
        [XmlAttribute(AttributeName = "Children")]
        public string Children { get; set; }
    }
}
