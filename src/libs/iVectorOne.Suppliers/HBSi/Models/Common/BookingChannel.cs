namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class BookingChannel
    {
        [XmlAttribute("Type")]
        public string ChannelType { get; set; } = string.Empty;

        [XmlAttribute("Primary")]
        public string Primary { get; set; } = string.Empty;

        [XmlElement("CompanyName")]
        public string CompanyName { get; set; } = string.Empty;
    }
}
