using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelBookingInfo")]
    public class HotelBookingInfo
    {
        public HotelBookingInfo(Price price, string hotelCode, string start, string end)
        {
            Price = price;
            HotelCode = hotelCode;
            Start = start;
            End = end;
        }

        public HotelBookingInfo()
        {
        }

        [XmlElement(ElementName = "Price")]
        public Price Price { get; set; }
        [XmlElement(ElementName = "HotelCode")]
        public string HotelCode { get; set; }
        [XmlAttribute(AttributeName = "Start")]
        public string Start { get; set; }
        [XmlAttribute(AttributeName = "End")]
        public string End { get; set; }
    }
}
