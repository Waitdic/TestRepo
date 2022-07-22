namespace iVectorOne.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Price
    {
        [XmlAttribute("amount")]
        public string Amount { get; set; } = string.Empty;

        [XmlAttribute("currencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}