namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class ProviderID
    {
        public ProviderID() { }

        [XmlAttribute("Provider")]
        public string Provider { get; set; } = string.Empty;
    }
}
