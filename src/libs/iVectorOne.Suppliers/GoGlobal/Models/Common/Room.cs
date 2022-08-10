namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Room
    {
        [XmlAttribute("Adults")]
        public string Adults { get; set; } = string.Empty;

        [XmlAttribute("RoomCount")]
        public string RoomCount { get; set; } = string.Empty;

        [XmlAttribute("ChildCount")]
        public string ChildCount { get; set; } = string.Empty;

        [XmlElement("ChildAge")]
        public List<string> ChildAges { get; set; } = new();
    }
}
