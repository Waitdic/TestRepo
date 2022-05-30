using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "ServiceTaxes")]
    public class ServiceTaxes
    {
        [XmlAttribute(AttributeName = "Included")]
        public string Included { get; set; }
        [XmlAttribute(AttributeName = "Amount")]
        public decimal Amount { get; set; }
    }
}
