namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class UniqueId
    {
        [XmlAttribute("ID")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("Type")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("CompanyName")]
        public string CompanyName { get; set; } = string.Empty;
        public bool ShouldSerializeCompanyName() => !string.IsNullOrEmpty(CompanyName);
    }
}
