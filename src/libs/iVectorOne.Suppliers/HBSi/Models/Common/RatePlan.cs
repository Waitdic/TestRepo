namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class RatePlan
    {
        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlAttribute("RatePlanType")]
        public string RatePlanType { get; set; } = string.Empty;
        public bool ShouldSerializeRatePlanType() => !string.IsNullOrEmpty(RatePlanType);

        [XmlElement("RatePlanDescription")]
        public RatePlanDescription RatePlanDescription { get; set; } = new();

        [XmlElement("MealsIncluded")]
        public MealsIncluded MealsIncluded { get; set; } = new();
    }
}
