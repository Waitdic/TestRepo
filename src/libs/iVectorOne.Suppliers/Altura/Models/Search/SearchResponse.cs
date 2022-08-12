namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class SearchResponse
    {
        public SearchResponse() { }

        [XmlArray("Hotels")]
        [XmlArrayItem("Hotel")]
        public List<Hotel> Hotels { get; set; } = new();

        [XmlAttribute("session")]
        public string Session { get; set; } = string.Empty;

        [XmlAttribute("type")]
        public string ResponseType { get; set; } = string.Empty;

        [XmlElement("Error")]
        public string ErrorInfo { get; set; } = string.Empty;
    }
}
