namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Accommodation
    {
        public string UID { get; set; } = string.Empty;

        [XmlArray("ROOMS")]
        [XmlArrayItem("Room")]
        public Room[] Rooms { get; set; } = Array.Empty<Room>();
    }
}
