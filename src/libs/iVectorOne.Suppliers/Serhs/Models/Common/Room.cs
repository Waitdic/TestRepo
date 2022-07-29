namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Room
    {
        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("adults")]
        public int Adults { get; set; }

        [XmlAttribute("children")]
        public int Children { get; set; }

        [XmlElement("child")]
        public List<GuestInfo> Child { get; set; } = new();

        [XmlElement("adult")]
        public List<GuestInfo> Adult { get; set; } = new();
    }
}