namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class CancelPenalty
    {
        [XmlAttribute("PolicyCode")]
        public string PolicyCode { get; set; } = string.Empty;

        [XmlAttribute("NonRefundable")]
        public bool NonRefundable { get; set; }

        [XmlElement("Deadline")]
        public Deadline Deadline { get; set; } = new();

        [XmlElement("AmountPercent")]
        public AmountPercent AmountPercent { get; set; } = new();
    }
}
