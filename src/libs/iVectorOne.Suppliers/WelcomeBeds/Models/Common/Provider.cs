namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Provider
    {
        public Provider() { }
        [XmlAttribute("Provider")]
        public string Name { get; set; } = string.Empty;

        [XmlArray(ElementName = "ProviderAreas")]
        [XmlArrayItem(ElementName = "Area")]
        public List<Area> ProviderAreas { get; set; } = new List<Area>();

        [XmlArray(ElementName = "Credentials")]
        [XmlArrayItem(ElementName = "Credential")]
        public List<Credential> Credentials { get; set; } = new List<Credential>();
    }
}
