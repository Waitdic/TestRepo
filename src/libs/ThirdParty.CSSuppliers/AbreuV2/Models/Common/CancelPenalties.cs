namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class CancelPenalties
    {
        [XmlAttribute("NonRefundable")]
        public string NonRefundable { get; set; } = string.Empty;

        [XmlElement("CancelPenalty")]
        public List<CancelPenalty> Penalties { get; set; } = new();
    }

    public class CancelPenalty
    {
        [XmlElement("Deadline")]
        public Deadline Deadline { get; set; } = new();

        [XmlElement("Charge")]
        public Charge Charge { get; set; } = new();
    }

    public class Deadline
    {
        [XmlAttribute("TimeUnit")]
        public string TimeUnit { get; set; } = string.Empty;
        [XmlAttribute("Units")]
        public string Units { get; set; } = string.Empty;
    }

    public class Charge
    {
        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;

        [XmlAttribute("Currency")]
        public string Currency { get; set; } = string.Empty;
    }
}
