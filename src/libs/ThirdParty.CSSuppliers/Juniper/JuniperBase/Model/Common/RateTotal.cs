namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class RateTotal
    {
        public RateTotal() { }

        [XmlAttribute("AmountAfterTax")]
        public string AmountAfterTax { get; set; } = string.Empty;

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
