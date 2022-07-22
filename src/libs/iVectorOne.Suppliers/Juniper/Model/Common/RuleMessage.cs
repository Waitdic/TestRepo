namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class RuleMessage
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        [XmlElement("StatusApplication")]
        public StatusApplication StatusApplication { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public RuleMessageExtension TpaExtension { get; set; } = new();
    }
}