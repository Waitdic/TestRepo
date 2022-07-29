namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class PolicyRule
    {
        [XmlAttribute("DateFrom")]
        public string DateFrom { get; set; } = string.Empty;

        [XmlAttribute("DateTo")]
        public string DateTo { get; set; } = string.Empty;

        [XmlAttribute("FixedPrice")]
        public string FixedPrice { get; set; } = string.Empty;

        [XmlAttribute("PercentPrice")]
        public string PercentPrice { get; set; } = string.Empty;

        [XmlAttribute("FirstNightPrice")]
        public string FirstNightPrice { get; set; } = string.Empty;

        [XmlAttribute("Nights")]
        public string Nights { get; set; } = string.Empty;
    }
}