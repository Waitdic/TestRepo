namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelAvailRq : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("Target")]
        public string Target { get; set; } = string.Empty;

        [XmlAttribute("TimeStamp")]
        public string TimeStamp { get; set; } = string.Empty;

        [XmlAttribute("BestOnly")]
        public string BestOnly { get; set; } = string.Empty;

        [XmlAttribute("SummaryOnly")]
        public string SummaryOnly { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlArray(ElementName = "AvailRequestSegments")]
        [XmlArrayItem(ElementName = "AvailRequestSegment")]
        public List<AvailRequestSegment> AvailRequestSegmets { get; set; } = new List<AvailRequestSegment>();

    }
}
