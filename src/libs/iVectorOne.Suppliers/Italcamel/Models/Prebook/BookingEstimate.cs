namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;
    using System.Xml.Serialization;

    public class BookingEstimate : SoapContent
    {
        [XmlElement("USERNAME")]
        public string Username { get; set; } = string.Empty;

        [XmlElement("PASSWORD")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("CITYUID")]
        public string CityUID { get; set; } = string.Empty;

        [XmlElement("LANGUAGEUID")]
        public string LanguageuId { get; set; } = string.Empty;

        [XmlElement("REQUEST")]
        public PrebookRequest Request { get; set; } = new();
    }
}
