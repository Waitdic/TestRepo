using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "AvailabilityRS")]
    public class AvailabilityRS
    {
        [XmlElement(ElementName = "Results")]
        public HotelAvailResults Results { get; set; }
        [XmlAttribute(AttributeName = "Url")]
        public string Url { get; set; }
        [XmlAttribute(AttributeName = "TimeStamp")]
        public string TimeStamp { get; set; }
        [XmlAttribute(AttributeName = "IntCode")]
        public string IntCode { get; set; }
    }
}
