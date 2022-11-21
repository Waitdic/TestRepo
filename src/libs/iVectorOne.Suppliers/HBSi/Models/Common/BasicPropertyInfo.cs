namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Linq;
    using System.Xml.Serialization;

    public class BasicPropertyInfo
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        [XmlAttribute("HotelName")]
        public string HotelName { get; set; } = string.Empty;

        [XmlAttribute("ChainCode")]
        public string ChainCode { get; set; } = string.Empty;

        [XmlAttribute("BrandCode")]
        public string BrandCode { get; set; } = string.Empty;

        public Address Address { get; set; } = new();
        public bool ShouldSerializeAddress() => Address.AddressLines.Any();
    }
}
