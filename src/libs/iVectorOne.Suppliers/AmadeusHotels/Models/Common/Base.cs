namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Base
    {
        [XmlAttribute("AmountAfterTax")]
        public decimal AmountAfterTax { get; set; }

        [XmlAttribute("AmountBeforeTax")]
        public decimal AmountBeforeTax { get; set; }

        [XmlAttribute]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlAttribute]
        public string RateTimeUnit { get; set; } = string.Empty;
    }
}
