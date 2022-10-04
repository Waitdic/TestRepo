namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomDetail
    {
        [XmlElement("ADULTS")]
        public int Adults { get; set; }

        [XmlElement("CHILDREN")]
        public int Children { get; set; }

        [XmlElement("CHILDAGE1")]
        public int ChildAge1 { get; set; }

        [XmlElement("CHILDAGE2")]
        public int ChildAge2 { get; set; }

        [XmlArray("BOARDS")]
        [XmlArrayItem("BOARD")]
        public Board[] Boards { get; set; } = Array.Empty<Board>();
    }
}
