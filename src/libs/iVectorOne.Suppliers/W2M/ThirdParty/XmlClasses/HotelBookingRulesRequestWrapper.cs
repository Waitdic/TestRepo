using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelBookingRulesRQ")]
    public class HotelBookingRulesRequestWrapper
    {
        public HotelBookingRulesRequestWrapper(Login login, HotelBookingRulesRequest hotelBookingRulesRequest,
            string version, string language)
        {
            Login = login;
            HotelBookingRulesRequest = hotelBookingRulesRequest;
            Version = version;
            Language = language;
        }

        public HotelBookingRulesRequestWrapper()
        {
        }

        [XmlElement(ElementName = "Login")]
        public Login Login { get; set; }
        [XmlElement(ElementName = "HotelBookingRulesRequest")]
        public HotelBookingRulesRequest HotelBookingRulesRequest { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
    }
}
