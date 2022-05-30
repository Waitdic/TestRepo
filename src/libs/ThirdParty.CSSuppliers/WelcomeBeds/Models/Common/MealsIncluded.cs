namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class MealsIncluded
    {
        public MealsIncluded() { }
        [XmlAttribute("MealPlanCodes")]
        public string MealPlanCodes { get; set; } = string.Empty;
    }

}
