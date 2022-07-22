namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomOption
    {
        [XmlAttribute("roomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlElement("roomDescription")]
        public string RoomDescription { get; set; } = string.Empty;

        [XmlElement("mealBasis")]
        public MealBasis MealBasis { get; set; } = new();

        [XmlElement("rateIdentifier")]
        public string RateIdentifier { get; set; } = string.Empty;

        [XmlElement("roomTotalPrice")]
        public RoomTotalPrice RoomTotalPrice { get; set; } = new();

        [XmlElement("availabilityStatus")]
        public bool AvailabilityStatus { get; set; }

        [XmlArray("cancellationPolicies")]
        [XmlArrayItem("cancellationPolicy")]
        public CancellationPolicy[] CancellationPolicies { get; set; } = Array.Empty<CancellationPolicy>();
    }
}
