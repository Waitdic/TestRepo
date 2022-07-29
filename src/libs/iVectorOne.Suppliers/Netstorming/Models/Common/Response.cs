namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Response
    {
        [XmlElement("search")]
        public Search Search { get; set; } = new();

        [XmlElement("nights")]
        public Nights Nights { get; set; } = new();

        [XmlElement("checkin")]
        public QueryDate Checkin { get; set; } = new();

        [XmlElement("checkout")]
        public QueryDate Checkout { get; set; } = new();

        [XmlElement("hotels")]
        public Hotels Hotels { get; set; } = new();

        [XmlAttribute("type")]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute("product")]
        public string Product { get; set; } = string.Empty;
    }
}