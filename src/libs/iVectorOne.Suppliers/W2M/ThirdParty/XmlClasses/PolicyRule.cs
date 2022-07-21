using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Rule")]
    public class PolicyRule
    {
        [XmlAttribute(AttributeName = "DateFrom")]
        public string DateFrom { get; set; }
        [XmlAttribute(AttributeName = "DateFromHour")]
        public string DateFromHour { get; set; }
        [XmlAttribute(AttributeName = "DateTo")]
        public string DateTo { get; set; }
        [XmlAttribute(AttributeName = "DateToHour")]
        public string DateToHour { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "FixedPrice")]
        public decimal FixedPrice { get; set; }
        [XmlAttribute(AttributeName = "PercentPrice")]
        public int PercentPrice { get; set; }
        [XmlAttribute(AttributeName = "Nights")]
        public int Nights { get; set; }
        [XmlAttribute(AttributeName = "ApplicationTypeNights")]
        public string ApplicationTypeNights { get; set; }
    }
}
