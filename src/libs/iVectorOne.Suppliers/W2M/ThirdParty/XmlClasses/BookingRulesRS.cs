using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "BookingRulesRS")]
    public class BookingRulesRS
    {
        [XmlElement(ElementName = "Results")]
        public Results Results { get; set; }
        [XmlAttribute(AttributeName = "Url")]
        public string Url { get; set; }
        [XmlAttribute(AttributeName = "TimeStamp")]
        public string TimeStamp { get; set; }
        [XmlAttribute(AttributeName = "IntCode")]
        public string IntCode { get; set; }
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }
    }
}
