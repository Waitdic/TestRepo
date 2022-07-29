using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Price")]
    public class Price
    {
        public Price(PriceRange priceRange)
        {
            PriceRange = priceRange;
        }

        public Price()
        {
        }

        [XmlElement(ElementName = "TotalFixAmounts")]
        public TotalFixAmounts TotalFixAmounts { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Currency")]
        public string Currency { get; set; }
        [XmlElement(ElementName = "PriceRange")]
        public PriceRange PriceRange { get; set; }
    }
}
