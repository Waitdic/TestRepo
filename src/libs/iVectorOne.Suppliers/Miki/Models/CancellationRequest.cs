namespace ThirdParty.CSSuppliers.Miki.Models
{
    using System.Xml.Serialization;
    using Common;

    public class CancellationRequest : SoapContent
    {
        [XmlAttribute("versionNumber")]
        public string VersionNumber { get; set; } = string.Empty;

        [XmlElement("requestAuditInfo")]
        public RequestAuditInfo RequestAuditInfo { get; set; } = new();

        [XmlElement("tourReference")]
        public string TourReference { get; set; } = string.Empty;
    }
}
