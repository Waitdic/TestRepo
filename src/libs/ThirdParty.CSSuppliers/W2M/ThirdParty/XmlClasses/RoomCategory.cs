using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "RoomCategory")]
    public class RoomCategory
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
