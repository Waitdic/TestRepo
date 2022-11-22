namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class StayDateRange
    {
        [XmlAttribute("Start")]
        public string Start { get; set; } = string.Empty;

        [XmlAttribute("End")]
        public string End { get; set; } = string.Empty;

        [XmlAttribute("Duration")]
        public string Duration { get; set; } = "Day";
    }
}
