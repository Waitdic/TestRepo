namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Criterion
    {
        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("code")]
        public string? Code { get; set; }

        [XmlAttribute("value")]
        public string? Value { get; set; }
    }
}