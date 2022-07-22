namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BookingRule
    {
        [XmlElement("TPA_Extensions")]
        public PreBookRuleExtension TpaExtensions { get; set; } = new();

        [XmlArray("CancelPenalties")]
        [XmlArrayItem("CancelPenalty")]
        public List<CancelPenalty> CancelPenalties { get; set; } = new();

        [XmlElement("Description")]
        public TextDescription Description { get; set; } = new();
    }
}