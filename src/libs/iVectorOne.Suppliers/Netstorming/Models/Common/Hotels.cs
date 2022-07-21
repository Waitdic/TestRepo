namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hotels
    {
        [XmlElement("hotel")]
        public Hotel[] Hotel { get; set; } = Array.Empty<Hotel>();

        [XmlAttribute("total")]
        public string Total { get; set; } = string.Empty;
    }
}