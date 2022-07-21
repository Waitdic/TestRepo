namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class Session
    {
        public Session() { }

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;
        public bool ShouldSerializeId() => !string.IsNullOrEmpty(Id);

        [XmlElement("AgencyId")]
        public string AgencyId { get; set; } = string.Empty;
        [XmlElement("Password")]
        public string Password { get; set; } = string.Empty;
        [XmlElement("ExternalId")]
        public string ExternalId { get; set; } = string.Empty;
    }
}
