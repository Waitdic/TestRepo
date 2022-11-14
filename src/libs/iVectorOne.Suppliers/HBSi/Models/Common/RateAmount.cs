namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class RateAmount
    {
        [XmlAttribute("AmountBeforeTax")]
        public string AmountBeforeTax { get; set; } = string.Empty;
        public bool ShouldSerializeAmountBeforeTax() => !string.IsNullOrEmpty(AmountBeforeTax);

        [XmlAttribute("AmountAfterTax")]
        public string AmountAfterTax { get; set; } = string.Empty;
        public bool ShouldSerializeAmountAfterTax() => !string.IsNullOrEmpty(AmountAfterTax);

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
