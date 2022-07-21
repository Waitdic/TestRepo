namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System.Xml.Serialization;

    public class Param
    {
        [XmlElement("hotls")]
        public Hotls Hotls { get; set; } = new();

        [XmlElement("id")]
        public string Id { get; set; } = string.Empty;
    }
}