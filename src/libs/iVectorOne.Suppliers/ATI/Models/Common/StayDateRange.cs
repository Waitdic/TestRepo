namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class StayDateRange
    {
        [XmlAttribute("Start")]
        public string Start { get; set; } = string.Empty;

        [XmlAttribute("Duration")]
        public string Duration { get; set; } = string.Empty;
    }
}
