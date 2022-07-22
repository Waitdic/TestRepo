using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelOffer")]
    public class HotelOffer
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
        [XmlAttribute(AttributeName = "End")]
        public string End { get; set; }
        [XmlAttribute(AttributeName = "RoomCategory")]
        public string RoomCategory { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "Category")]
        public string Category { get; set; }
    }
}
