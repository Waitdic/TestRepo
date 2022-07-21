namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class CancellationRequest
    {
        public CancellationRequest() { }

        [XmlAttribute("type")]
        public string RequestType { get; set; } = Constant.RequestTypeCancellation;

        [XmlAttribute("version")]
        public string Version { get; set; } = Constant.ApiVersion;

        [XmlElement("Session")]
        public Session Session { get; set; } = new();

        [XmlElement("Cancellation")]
        public Cancellation Cancellation { get; set; } = new();
    }
}
