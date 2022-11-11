namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class RoomDescription
    {
        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }

}
