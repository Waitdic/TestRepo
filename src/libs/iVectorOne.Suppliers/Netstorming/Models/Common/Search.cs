namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Search
    {
        [XmlAttribute("number")]
        public string Number { get; set; } = string.Empty;

        [XmlAttribute("time")]
        public string Time { get; set; } = string.Empty;
    }
}