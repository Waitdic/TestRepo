namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class MealBasis
    {
        [XmlAttribute("mealBasisCode")]
        public string MealBasisCode { get; set; } = string.Empty;
    }
}
