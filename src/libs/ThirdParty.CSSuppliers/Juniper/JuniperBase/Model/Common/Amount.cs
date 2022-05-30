using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class Amount
    {
        public Amount() { }

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlText]
        public string Content { get; set; } = string.Empty;
    }
}
