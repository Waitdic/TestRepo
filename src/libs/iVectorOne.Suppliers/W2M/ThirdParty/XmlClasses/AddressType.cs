using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Address")]
    public class AddressType
    {
        [XmlElement(ElementName = "Address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "Latitude")]
        public string Latitude { get; set; }
        [XmlElement(ElementName = "Longitude")]
        public string Longitude { get; set; }
    }
}
