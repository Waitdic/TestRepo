namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class OTA_HotelBookingRuleRQ
    {
        [XmlAttribute("PrimaryLangID")]
        public string PrimaryLangId { get; set; } = string.Empty;

        [XmlAttribute("SequenceNmbr")]
        public string SequenceNmbr { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlElement("RuleMessage")]
        public RuleMessage RuleMessage { get; set; } = new();
    }
}