namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Pax
    {
        [XmlElement("hab")]
        public Hab[] Hab { get; set; } = Array.Empty<Hab>();


        [XmlAttribute("cod")]
        public string Cod { get; set; } = string.Empty;
    }
}