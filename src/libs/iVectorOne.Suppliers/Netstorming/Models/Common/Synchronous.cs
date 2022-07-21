namespace ThirdParty.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Synchronous
    {
        [XmlAttribute("value")]
        public string Value { get; set; } = string.Empty;
    }
}