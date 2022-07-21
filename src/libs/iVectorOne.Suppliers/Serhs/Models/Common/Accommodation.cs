namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Accommodation
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlArray("concepts")]
        [XmlArrayItem("concept")]
        public List<Concept> Concepts { get; set; } = new();
    }
}
