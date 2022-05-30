using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class RateExtension
    {
        public RateExtension() { }

        [XmlElement("DailyBreakdown")]
        public string DailyBreakdown { get; set; } = string.Empty;

        [XmlElement("MandatoryBookingRule")]
        public string MandatoryBookingRule { get; set; } = string.Empty;

        [XmlElement("RoomCategory")]
        public RoomCategory RoomCategory { get; set; } = new();

        [XmlElement("Mealplan")]
        public MealPlan MealPlan { get; set; } = new();
    }
}
