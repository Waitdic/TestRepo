namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class PrebookRequest
    {
        [XmlAttribute("type")]
        public string RequestType { get; set; } = Constant.RequestTypePrebook;

        [XmlAttribute("version")]
        public string Version { get; set; } = Constant.ApiVersion;

        [XmlElement("Session")]
        public Session Session { get; set; } = new();

        [XmlElement("RateId")]
        public string RateId { get; set; } = string.Empty;
    }
}
