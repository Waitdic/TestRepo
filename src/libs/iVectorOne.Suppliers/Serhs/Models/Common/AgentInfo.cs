namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class AgentInfo
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("email")]
        public string? Email { get; set; }

        [XmlAttribute("phone")]
        public string? Phone { get; set; }
    }
}
