using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Rule")]
    public class Rule
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
        public string FixedPrice { get; set; }
        [XmlAttribute(AttributeName = "PercentPrice")]
        public string PercentPrice { get; set; }
        [XmlAttribute(AttributeName = "Nights")]
        public string Nights { get; set; }
        [XmlAttribute(AttributeName = "ApplicationTypeNights")]
        public string ApplicationTypeNights { get; set; }
    }
}
