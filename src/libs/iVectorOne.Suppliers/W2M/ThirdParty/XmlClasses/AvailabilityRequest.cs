using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelCheckAvailRQ")]
    public class AvailabilityRequest
    {
        public AvailabilityRequest(Login login, CheckAvailabilityRequest checkAvailabilityRequest, string version,
            string language)
        {
            Login = login;
            CheckAvailabilityRequest = checkAvailabilityRequest;
            Version = version;
            Language = language;
        }

        public AvailabilityRequest()
        {
        }

        [XmlElement(ElementName = "Login")]
        public Login Login { get; set; }
        [XmlElement(ElementName = "HotelCheckAvailRequest")]
        public CheckAvailabilityRequest CheckAvailabilityRequest { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
    }
}
