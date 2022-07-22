using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "PriceRange")]
    public class PriceRange
    {
        public PriceRange(decimal minimum, decimal maximum, string currency)
        {
            Minimum = minimum;
            Maximum = maximum;
            Currency = currency;
        }

        public PriceRange()
        {
        }

        [XmlAttribute(AttributeName = "Minimum")]
        public decimal Minimum { get; set; }
        [XmlAttribute(AttributeName = "Maximum")]
        public decimal Maximum { get; set; }
        [XmlAttribute(AttributeName = "Currency")]
        public string Currency { get; set; }
    }
}
