namespace ThirdParty.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationDeadline
    {
        [XmlAttribute("timeUnit")]
        public string TimeUnit { get; set; } = string.Empty;

        [XmlAttribute("effectMoment")]
        public string EffectMoment { get; set; } = string.Empty;

        [XmlAttribute("amount")]
        public int Amount { get; set; }
    }
}