namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlArray("Rates")]
        [XmlArrayItem("Rate")]
        public List<Rate> Rates { get; set; } = new();

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Resort")]
        public string Resort { get; set; } = string.Empty;
    }
}
