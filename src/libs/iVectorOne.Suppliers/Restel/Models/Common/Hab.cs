namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hab
    {
        [XmlElement("reg")]
        public Reg[] Reg { get; set; } = Array.Empty<Reg>();

        [XmlAttribute("cod")]
        public string Cod { get; set; } = string.Empty;

        [XmlAttribute("desc")]
        public string Desc { get; set; } = string.Empty;
    }
}