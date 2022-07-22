using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelBookingRQ")]
    public class BookingRequest
    {
        public BookingRequest(Login login, Paxes paxes, Holder holder, string externalBookingReference,
            Comments comments, Elements elements, string version, string language)
        {
            Login = login;
            Paxes = paxes;
            Holder = holder;
            ExternalBookingReference = externalBookingReference;
            Comments = comments;
            Elements = elements;
            Version = version;
            Language = language;
        }

        public BookingRequest()
        {
        }

        [XmlElement(ElementName = "Login")]
        public Login Login { get; set; }
        [XmlElement(ElementName = "Paxes")]
        public Paxes Paxes { get; set; }
        [XmlElement(ElementName = "Holder")]
        public Holder Holder { get; set; }
        [XmlElement(ElementName = "ExternalBookingReference")]
        public string ExternalBookingReference { get; set; }
        [XmlElement(ElementName = "Comments")]
        public Comments Comments { get; set; }
        [XmlElement(ElementName = "Elements")]
        public Elements Elements { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
    }
}
