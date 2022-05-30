using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "AgencyData")]
    public class AgencyData
    {
        [XmlElement(ElementName = "ReferencedAgency")]
        public string ReferencedAgency { get; set; }
        [XmlElement(ElementName = "AgencyCode")]
        public string AgencyCode { get; set; }
        [XmlElement(ElementName = "AgencyName")]
        public string AgencyName { get; set; }
        [XmlElement(ElementName = "AgencyHandledBy")]
        public string AgencyHandledBy { get; set; }
        [XmlElement(ElementName = "AgencyEmail")]
        public string AgencyEmail { get; set; }
    }
}
