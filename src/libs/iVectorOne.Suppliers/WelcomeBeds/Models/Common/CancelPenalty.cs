namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class CancelPenalty
    {
        public CancelPenalty() { }

        [XmlAttribute("Start")]
        public string Start { get; set; } = string.Empty;

        [XmlAttribute("End")]
        public string End { get; set; } = string.Empty;

        [XmlAttribute("NonRefundable")]
        public string NonRefundable { get; set; } = string.Empty;

        [XmlElement("AmountPercent")]
        public AmountPercent AmountPercent { get; set; } = new();
    }

    public class AmountPercent
    {
        public AmountPercent() { }

        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;
    }
}
