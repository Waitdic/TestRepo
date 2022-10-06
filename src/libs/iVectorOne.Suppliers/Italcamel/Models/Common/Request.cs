namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Request
    {
        [XmlElement("CHECKIN")]
        public string CheckIn { get; set; } = string.Empty;

        [XmlElement("CHECKOUT")]
        public string CheckOut { get; set; } = string.Empty;

        [XmlElement("ACCOMMODATIONUID")]
        public string AccomodationuId { get; set; } = string.Empty;
        public bool ShouldSerializeAccomodationuId() => !string.IsNullOrEmpty(AccomodationuId);
    }
}
