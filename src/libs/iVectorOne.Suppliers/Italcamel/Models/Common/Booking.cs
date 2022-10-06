namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Booking
    {
        [XmlElement("STATUS")]
        public int Status { get; set; }

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public PrebookRoom[] Rooms { get; set; } = Array.Empty<PrebookRoom>();

        [XmlElement("UID")]
        public string UID { get; set; } = string.Empty;

        [XmlElement("NUMBER")]
        public string Number { get; set; } = string.Empty;
    }
}
