namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class MealPlan
    {
        public MealPlan() { }

        [XmlAttribute("Category")]
        public string Category { get; set; } = string.Empty;

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlText]
        public string Content { get; set; } = string.Empty;
    }
}
