namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class Rate
    {
        [XmlAttribute]
        public decimal TotalCost { get; set; }

        [XmlAttribute]
        public string Currency { get; set; } = string.Empty;

        [XmlAttribute]
        public decimal VATRate { get; set; }
    }
}
