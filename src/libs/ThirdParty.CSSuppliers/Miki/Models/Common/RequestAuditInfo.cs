namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class RequestAuditInfo
    {
        [XmlElement("agentCode")]
        public string AgentCode { get; set; } = string.Empty;

        [XmlElement("requestPassword")]
        public string RequestPassword { get; set; } = string.Empty;

        [XmlElement("requestID")]
        public string RequestID { get; set; } = string.Empty;

        [XmlElement("requestDateTime")]
        public string RequestDateTime { get; set; } = string.Empty;
    }
}
