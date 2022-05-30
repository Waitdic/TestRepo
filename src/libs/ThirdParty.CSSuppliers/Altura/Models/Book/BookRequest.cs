namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class BookRequest
    {
        public BookRequest() { }

        [XmlAttribute("type")]
        public string RequestType { get; set; } = Constant.RequestTypeBook;

        [XmlAttribute("version")]
        public string Version { get; set; } = Constant.ApiVersion;

        [XmlElement("Session")]
        public Session Session { get; set; } = new();

        [XmlElement("BookingConfirmation")]
        public BookingConfirmation BookingConfirmation { get; set; } = new();

        [XmlElement("Guest")]
        public Guest Guest { get; set; } = new();
    }
}
