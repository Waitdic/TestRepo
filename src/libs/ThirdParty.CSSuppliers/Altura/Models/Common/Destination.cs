namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class Destination
    {
        public Destination() { }

        [XmlAttribute("type")]
        public string DestinationType { get; set; }

        [XmlText]
        public string Content { get; set; }
    }
}
