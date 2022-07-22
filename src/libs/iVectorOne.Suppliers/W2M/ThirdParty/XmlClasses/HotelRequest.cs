using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelRequest")]
    public class HotelRequest
    {
        public HotelRequest(SearchSegmentsHotels searchSegmentsHotels, RelativePaxesDistribution relativePaxesDistribution)
        {
            SearchSegmentsHotels = searchSegmentsHotels;
            RelativePaxesDistribution = relativePaxesDistribution;
        }

        public HotelRequest()
        {
        }

        [XmlElement(ElementName = "SearchSegmentsHotels")]
        public SearchSegmentsHotels SearchSegmentsHotels { get; set; }
        [XmlElement(ElementName = "RelPaxesDist")]
        public RelativePaxesDistribution RelativePaxesDistribution { get; set; }
    }
}
