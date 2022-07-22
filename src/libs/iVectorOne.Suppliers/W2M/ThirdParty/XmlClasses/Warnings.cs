using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Warnings")]
    public class Warnings
    {
        [XmlElement(ElementName = "Warning")]
        public Warning Warning { get; set; }

        [XmlElement(ElementName = "CancelInfo")]
        public CancelInfo CancelInfo { get; set; }
    }
}
