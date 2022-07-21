using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "SearchSegmentsHotels")]
    public class SearchSegmentsHotels
    {
        public SearchSegmentsHotels(SearchSegmentHotels searchSegmentsHotels, HotelCodes hotelCodes, string countryOfResidence)
        {
            SearchSegmentHotels = searchSegmentsHotels;
            HotelCodes = hotelCodes;
            CountryOfResidence = countryOfResidence;
        }

        public SearchSegmentsHotels()
        {
        }

        [XmlElement(ElementName = "SearchSegmentHotels")]
        public SearchSegmentHotels SearchSegmentHotels { get; set; }
        [XmlElement(ElementName = "HotelCodes")]
        public HotelCodes HotelCodes { get; set; }
        [XmlElement(ElementName = "CountryOfResidence")]
        public string CountryOfResidence { get; set; }
    }
}
