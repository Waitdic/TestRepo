namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Concept
    {
        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("code")]
        public string? Code { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("standardCode")]
        public string StandardCode { get; set; } = string.Empty;

        [XmlArray("boards")]
        [XmlArrayItem("board")]
        public List<Board> Boards { get; set; } = new();
    }
}