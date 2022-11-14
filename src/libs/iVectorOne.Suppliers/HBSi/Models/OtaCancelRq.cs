namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class OtaCancelRq : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("Target")]
        public string Target { get; set; } = string.Empty;

        [XmlAttribute("TimeStamp")]
        public string TimeStamp { get; set; } = string.Empty;

        [XmlAttribute("CancelType")]
        public string CancelType { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();

        [XmlElement("Verification")]
        public Verification Verification { get; set; } = new();
    }
}
