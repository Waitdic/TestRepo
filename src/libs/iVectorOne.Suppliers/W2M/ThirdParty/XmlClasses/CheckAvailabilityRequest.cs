using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelCheckAvailRequest")]
    public class CheckAvailabilityRequest
    {
        public CheckAvailabilityRequest(CheckAvailabilityHotelOption checkAvailabilityHotelOption,
            SearchSegmentsHotels searchSegmentsHotels)
        {
            CheckAvailabilityHotelOption = checkAvailabilityHotelOption;
            SearchSegmentsHotels = searchSegmentsHotels;
        }

        public CheckAvailabilityRequest()
        {
        }

        [XmlElement(ElementName = "HotelOption")]
        public CheckAvailabilityHotelOption CheckAvailabilityHotelOption { get; set; }
        [XmlElement(ElementName = "SearchSegmentsHotels")]
        public SearchSegmentsHotels SearchSegmentsHotels { get; set; }
    }
}
