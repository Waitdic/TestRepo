namespace ThirdParty.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationPenalty
    {
        [XmlAttribute("penaltyUnit")]
        public string PenaltyUnit { get; set; } = string.Empty;

        [XmlAttribute("amount")]
        public decimal Amount { get; set; }
    }
}