using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Errors")]
    public class Errors
    {
        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
    }
}
