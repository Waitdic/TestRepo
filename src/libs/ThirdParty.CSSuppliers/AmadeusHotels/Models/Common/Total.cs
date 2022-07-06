namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Total
    {
        [XmlAttribute("AmountAfterTax")]
        public decimal AmountAfterTax { get; set; }

        [XmlAttribute("AmountBeforeTax")]
        public decimal AmountBeforeTax { get; set; }

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
