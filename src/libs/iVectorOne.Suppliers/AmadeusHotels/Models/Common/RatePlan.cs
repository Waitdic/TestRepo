namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RatePlan
    {
        public MealsIncluded MealsIncluded { get; set; } = new();

        [XmlArray("CancelPenalties")]
        [XmlArrayItem("CancelPenalty")]
        public CancelPenalty[]? CancelPenalties { get; set; }
        public bool ShouldSerializeCancelPenalties() => CancelPenalties != null;

        public Commission Commission { get; set; } = new();
    }
}
