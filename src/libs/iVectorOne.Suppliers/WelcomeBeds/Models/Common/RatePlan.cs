namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class RatePlan
    {
        public RatePlan() { }

        [XmlElement("MealsIncluded")]
        public MealsIncluded MealsIncluded { get; set; } = new MealsIncluded();
        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;
        [XmlAttribute("RatePlanName")]
        public string RatePlanName { get; set; } = string.Empty;
    }

}
