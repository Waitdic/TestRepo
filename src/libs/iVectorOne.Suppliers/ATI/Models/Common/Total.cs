namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Total
    {
        [XmlAttribute("AmountAfterTax")]
        public decimal AmountAfterTax { get; set; }

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
