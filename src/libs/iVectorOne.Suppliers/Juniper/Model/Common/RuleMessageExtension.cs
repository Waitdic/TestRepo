namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class RuleMessageExtension
    {
        [XmlElement("ForceCurrency")]
        public string ForceCurrency { get; set; } = string.Empty;
        [XmlElement("ShowSupplements")]
        public string ShowSupplements { get; set; } = string.Empty;
        [XmlElement("PaxCountry")]
        public string PaxCountry { get; set; } = string.Empty;
    }
}