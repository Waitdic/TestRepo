namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class MealsIncluded
    {
        [XmlAttribute("MealPlanCodes")]
        public string MealPlanCodes { get; set; } = string.Empty;
    }
}
