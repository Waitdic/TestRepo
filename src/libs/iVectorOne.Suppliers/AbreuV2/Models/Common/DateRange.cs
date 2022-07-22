namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class DateRange
    {
        [XmlAttribute("Start")]
        public string Start { get; set; } = string.Empty;
        [XmlAttribute("End")]
        public string End { get; set; } = string.Empty;
    }
}
