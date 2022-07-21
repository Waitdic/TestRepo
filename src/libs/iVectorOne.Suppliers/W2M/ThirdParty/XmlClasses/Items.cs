using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Items")]
    public class Items
    {
        [XmlElement(ElementName = "HotelItem")]
        public HotelItem HotelItem { get; set; }
    }
}
