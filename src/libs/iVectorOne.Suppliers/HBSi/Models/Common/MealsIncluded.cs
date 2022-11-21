namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class MealsIncluded
    {
        [XmlAttribute("MealPlanCodes")]
        public string MealPlanCodes { get; set; } = string.Empty;
    }

}
