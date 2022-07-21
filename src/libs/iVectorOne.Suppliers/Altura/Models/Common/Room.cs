using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Models.Altura
{
    public class Room
    {
        [XmlAttribute("id")]
        public string RoomId { get; set; } = string.Empty;
        [XmlAttribute("Code")]
        public int Code { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Price")]
        public decimal Price { get; set; }
        [XmlAttribute("currency")]
        public string Currency { get; set; }
        [XmlAttribute("Mapping")]
        public string Mapping { get; set; }
    }
}
