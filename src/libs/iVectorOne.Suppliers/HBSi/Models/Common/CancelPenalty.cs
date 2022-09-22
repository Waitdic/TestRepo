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

    public class Deadline
    {
        [XmlAttribute("AbsoluteDeadline")]
        public string AbsoluteDeadline { get; set; } = string.Empty;
    }

    public class AmountPercent
    {
        [XmlAttribute("Percent")]
        public string Percent { get; set; } = string.Empty;

        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;

        [XmlAttribute("NmbrOfNights")]
        public string NmbrOfNights { get; set; } = string.Empty;
    }
}
