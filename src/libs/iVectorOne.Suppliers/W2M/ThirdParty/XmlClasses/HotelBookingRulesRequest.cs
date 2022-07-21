using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelBookingRulesRequest")]
    public class HotelBookingRulesRequest
    {
        public HotelBookingRulesRequest(RequestHotelOption hotelOption, SearchSegmentsHotels searchSegmentsHotels)
        {
            HotelOption = hotelOption;
            SearchSegmentsHotels = searchSegmentsHotels;
        }

        public HotelBookingRulesRequest()
        {
        }

        [XmlElement(ElementName = "HotelOption")]
        public RequestHotelOption HotelOption { get; set; }
        [XmlElement(ElementName = "SearchSegmentsHotels")]
        public SearchSegmentsHotels SearchSegmentsHotels { get; set; }
    }
}
