namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class OtaCancelRq : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("CancelType")]
        public string CancelType { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new();
    }
}
