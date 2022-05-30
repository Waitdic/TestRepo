namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class AmountDetails
    {
        [XmlElement("total_amount")]
        public List<string> TotalAmount { get; set; } = new();

        [XmlAttribute("currencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
