namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Period
    {
        [XmlAttribute("start")]
        public string? Start { get; set; }

        [XmlAttribute("end")]
        public string? End { get; set; }
    }
}