using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "CancellationPolicy")]
    public class CancellationPolicy
    {
        [XmlElement(ElementName = "FirstDayCostCancellation")]
        public FirstDayCostCancellation FirstDayCostCancellation { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "PolicyRules")]
        public PolicyRules PolicyRules { get; set; }
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }
}
