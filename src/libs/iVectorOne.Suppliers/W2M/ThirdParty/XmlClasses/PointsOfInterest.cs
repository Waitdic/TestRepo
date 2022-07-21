using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "PointsOfInterest")]
    public class PointsOfInterest
    {
        [XmlElement(ElementName = "PointOfInterest")]
        public PointOfInterest PointOfInterest { get; set; }
    }
}
