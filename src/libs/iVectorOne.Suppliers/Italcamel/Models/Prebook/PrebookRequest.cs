namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class PrebookRequest : Request
    {
        public string UID { get; set; } = string.Empty;

        [XmlElement("TYPE")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("NOTES")]
        public string Notes { get; set; } = string.Empty;

        [XmlElement("REFERENCENUMBER")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public PrebookRoom[] Rooms { get; set; } = Array.Empty<PrebookRoom>();
    }
}
