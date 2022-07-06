namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class ResponseAuditInfo
    {
        [XmlElement("agentCode")]
        public string AgentCode { get; set; } = string.Empty;
    }
}
