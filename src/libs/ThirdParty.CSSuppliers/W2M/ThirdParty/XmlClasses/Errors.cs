using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Errors")]
    public class Errors
    {
        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
    }
}
