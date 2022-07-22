namespace iVectorOne.CSSuppliers.Serhs.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("request")]
    public class SerhsCancellationRequest : BaseRequest
    {
        public SerhsCancellationRequest() { }

        public SerhsCancellationRequest(string version, string clientCode, string password, string branch, string tradingGroup,
            string languageCode) : base(version, clientCode, password, branch, tradingGroup, languageCode) { }

        [XmlAttribute("type")]
        public override string Type { get; set; } = "CANCEL";

        [XmlAttribute("sessionid")]
        public string SessionId { get; set; } = "xxx";

        [XmlElement("booking")]
        public Booking Booking { get; set; } = new();

        [XmlElement("agentInfo")]
        public AgentInfo AgentInfo { get; set; } = new();
    }
}
