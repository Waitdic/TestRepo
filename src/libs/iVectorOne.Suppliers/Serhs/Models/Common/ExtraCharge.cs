namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ExtraCharge
    {
        [XmlAttribute("obligatory")]
        public string Obligatory { get; set; } = string.Empty;

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlArray("prices")]
        [XmlArrayItem("price")]
        public List<Price> Prices { get; set; } = new();
    }
}
