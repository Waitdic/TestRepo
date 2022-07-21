namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Rate
    {
        [XmlAttribute("RateTimeUnit")]
        public string RateTimeUnit { get; set; } = string.Empty;

        [XmlAttribute("EffectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;

        [XmlAttribute("ExpireDate")]
        public string ExpireDate { get; set; } = string.Empty;

        public PaymentPolicies PaymentPolicies { get; set; } = new();

        public Base Base { get; set; } = new();
    }
}
