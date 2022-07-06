namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class Total
    {
        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;

        [XmlAttribute("Currency")]
        public string Currency { get; set; } = string.Empty;
    }
}
