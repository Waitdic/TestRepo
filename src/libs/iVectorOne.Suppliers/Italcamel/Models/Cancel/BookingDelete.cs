namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;
    using System.Xml.Serialization;

    public class BookingDelete : SoapContent
    {
        [XmlElement("USERNAME")]
        public string Username { get; set; } = string.Empty;

        [XmlElement("PASSWORD")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("BOOKINGUID")]
        public string BookinguId { get; set; } = string.Empty;

        [XmlElement("LANGUAGEUID")]
        public string LanguageuId { get; set; } = string.Empty;
    }
}
