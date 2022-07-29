using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Prices")]
    public class Prices
    {
        [XmlElement(ElementName = "Price")]
        public Price Price { get; set; }
    }
}
