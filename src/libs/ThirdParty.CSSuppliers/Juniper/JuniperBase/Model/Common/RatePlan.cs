namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class RatePlan
    {
        public RatePlan() { }

        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;
    }
}
