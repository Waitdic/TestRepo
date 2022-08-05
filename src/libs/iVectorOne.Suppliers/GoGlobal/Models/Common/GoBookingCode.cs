namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Xml.Serialization;

    public class GoBookingCode
    {
        [XmlAttribute("Status")]
        public string Status { get; set; } = string.Empty;

        [XmlAttribute("GoReference")]
        public string GoReference { get; set; } = string.Empty;

        [XmlAttribute("TotalPrice")]
        public string TotalPrice { get; set; } = string.Empty;

        [XmlAttribute("Currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlAttribute("Service")]
        public string Service { get; set; } = string.Empty;

        [XmlText]
        public string Content { get; set; } = string.Empty;
    }
}
