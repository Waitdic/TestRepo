using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "PriceInformation")]
    public class PriceInformation
    {
        [XmlElement(ElementName = "Board")]
        public Board Board { get; set; }
        [XmlElement(ElementName = "HotelRooms")]
        public HotelRooms HotelRooms { get; set; }
        [XmlElement(ElementName = "Prices")]
        public Prices Prices { get; set; }
        [XmlElement(ElementName = "HotelContent")]
        public HotelContent HotelContent { get; set; }
    }
}
