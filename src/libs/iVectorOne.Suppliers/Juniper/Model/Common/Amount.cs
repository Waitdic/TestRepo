namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class Amount
    {
        public Amount() { }

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlText]
        public string Content { get; set; } = string.Empty;
    }
}