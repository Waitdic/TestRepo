using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class RateDescription
    {
        public RateDescription() { }

        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}
