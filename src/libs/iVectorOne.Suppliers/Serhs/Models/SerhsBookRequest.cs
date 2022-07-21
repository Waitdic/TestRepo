namespace iVectorOne.Suppliers.Serhs.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("request")]
    public class SerhsBookRequest : BaseRequest
    {
        public SerhsBookRequest() { }

        public SerhsBookRequest(string version, string clientCode, string password, string branch, string tradingGroup,
            string languageCode) : base(version, clientCode, password, branch, tradingGroup, languageCode) { }

        [XmlAttribute("type")]
        public override string Type { get; set; } = "CONFIRM";

        [XmlElement("customer")]
        public Customer Customer { get; set; } = new();

        [XmlElement("address")]
        public string? Address { get; set; }

        [XmlElement("city")]
        public City City { get; set; } = new();

        [XmlElement("region")]
        public Region Region { get; set; } = new();

        [XmlElement("document")]
        public Document Document { get; set; } = new();

        [XmlElement("contactInfo")]
        public ContactInfo ContactInfo { get; set; } = new();

        [XmlElement("preBooking")]
        public Booking PreBooking { get; set; } = new();
    }
}
