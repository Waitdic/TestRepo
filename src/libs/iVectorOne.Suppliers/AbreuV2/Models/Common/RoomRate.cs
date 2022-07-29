namespace iVectorOne.Suppliers.AbreuV2.Models
{
    using System.Linq;
    using System.Xml.Serialization;

    public class RoomRate
    {
        [XmlAttribute("MealPlan")]
        public string MealPlan { get; set; } = string.Empty;
        public bool ShouldSerializeMealPlan() => !string.IsNullOrEmpty(MealPlan);

        [XmlAttribute("BookingCode")]
        public string BookingCode { get; set; } = string.Empty;
        public bool ShouldSerializeBookingCode() => !string.IsNullOrEmpty(BookingCode);

        [XmlElement("CancelPenalties")]
        public CancelPenalties CancelPenalties { get; set; } = new();
        public bool ShouldSerializeCancelPenalties() => CancelPenalties?.Penalties?.Any() ?? false;

        [XmlElement("Total")]
        public Total Total { get; set; } = new();
        public bool ShouldSerializeTotal() => !string.IsNullOrEmpty(Total?.Currency);
    }
}
