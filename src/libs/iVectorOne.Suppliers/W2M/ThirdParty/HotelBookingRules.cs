using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelBookingRules")]
    public class HotelBookingRules
    {
        public HotelBookingRules(HotelBookingRulesRequestWrapper hotelBookingRulesRequest)
        {
            HotelBookingRulesRequest = hotelBookingRulesRequest;
        }

        public HotelBookingRules()
        {
        }

        [XmlElement(ElementName = "HotelBookingRulesRQ")]
        public HotelBookingRulesRequestWrapper HotelBookingRulesRequest { get; set; }
    }
}
