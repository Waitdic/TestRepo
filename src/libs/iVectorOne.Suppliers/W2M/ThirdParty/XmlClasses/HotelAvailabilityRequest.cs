using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelAvailRQ")]
    public class HotelAvailabilityRequest
    {
        public HotelAvailabilityRequest(Login login, Paxes paxes, HotelRequest hotelRequest, 
            AvailabilityAdvancedOptions advancedOptions, string version, string language)
        {
            Login = login;
            Paxes = paxes;
            HotelRequest = hotelRequest;
            AdvancedOptions = advancedOptions;
            Version = version;
            Language = language;
        }

        public HotelAvailabilityRequest()
        {
        }

        [XmlElement(ElementName = "Login")]
        public Login Login { get; set; }
        [XmlElement(ElementName = "Paxes")]
        public Paxes Paxes { get; set; }
        [XmlElement(ElementName = "HotelRequest")]
        public HotelRequest HotelRequest { get; set; }
        [XmlElement(ElementName = "AdvancedOptions")]
        public AvailabilityAdvancedOptions AdvancedOptions { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
    }
}
