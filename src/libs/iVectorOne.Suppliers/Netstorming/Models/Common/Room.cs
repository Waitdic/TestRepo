namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Room
    {
        [XmlAttribute("type")]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute("required")]
        public int Required { get; set; }

        [XmlAttribute("extrabed")]
        public string Extrabed { get; set; } = "false";

        public bool ShouldSerializeExtrabed() => Extrabed != "false";

        [XmlAttribute("cot")]
        public string Cot { get; set; } = "false";

        public bool ShouldSerializeCot() => Cot != "false";

        [XmlAttribute("age")]
        public int Age { get; set; }

        public bool ShouldSerializeAge() => Age > 0;

        [XmlElement("pax")]
        public Pax[] Pax { get; set; } = Array.Empty<Pax>();

        [XmlAttribute("occupancy")]
        public string? Occupancy { get; set; }

        [XmlElement("price")]
        public Price[] Price { get; set; } = Array.Empty<Price>();
    }
}