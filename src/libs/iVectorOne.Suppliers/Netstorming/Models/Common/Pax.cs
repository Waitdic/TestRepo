namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Pax
    {
        [XmlAttribute("leader")]
        public string Leader { get; set; } = string.Empty;

        [XmlAttribute("title")]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("surname")]
        public string Surname { get; set; } = string.Empty;

        [XmlAttribute("initial")]
        public string Initial { get; set; } = string.Empty;

        [XmlAttribute("age")]
        public int Age { get; set; }

        public bool ShouldSerializeAge() => Age > 0;
    }
}