namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class ExpectedPriceRange
    {
        public ExpectedPriceRange() { }

        [XmlAttribute("min")]
        public string Min { get; set; } = string.Empty;
        [XmlAttribute("max")]
        public string Max { get; set; } = string.Empty;
    }
}