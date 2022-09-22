namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class RoomStay
    {
        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> RoomTypes { get; set; } = new();

        [XmlArray("RatePlans")]
        [XmlArrayItem("RatePlan")]
        public List<RatePlan> RatePlans { get; set; } = new();

        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public List<RoomRate> RoomRates { get; set; } = new();

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public List<GuestCount> GuestCounts { get; set; } = new();
        public bool ShouldSerializeGuestCounts() => GuestCounts != null && GuestCounts.Any();

        [XmlElement("TimeSpan")]
        public StayTimeSpan TimeSpan { get; set; } = new();
        public bool ShouldSerializeTimeSpan() => !string.IsNullOrEmpty(TimeSpan.Start);

        [XmlArray("CancelPenalties")]
        [XmlArrayItem("CancelPenalty")]
        public List<CancelPenalty> CancelPenalties { get; set; } = new();
        public bool ShouldSerializeGuestCancelPenalties() => CancelPenalties != null && CancelPenalties.Any();

        [XmlElement("Total")]
        public RateAmount Total { get; set; } = new();
        public bool ShouldSerializeTotal() => !string.IsNullOrEmpty(Total.AmountAfterTax);

        [XmlElement("BasicPropertyInfo")]
        public BasicPropertyInfo BasicPropertyInfo { get; set; } = new();
        public bool ShouldSerializeBasicPropertyInfo() => !string.IsNullOrEmpty(BasicPropertyInfo.HotelCode);

        [XmlArray("ResGuestRPHs")]
        [XmlArrayItem("ResGuestRPH")]
        public List<ResGuestRPH> ResGuestRPHs { get; set; } = new();
        public bool ShouldSerializeResGuestRPHs() => ResGuestRPHs != null && ResGuestRPHs.Any();
    }

    public class ResGuestRPH
    {
        [XmlAttribute("RPH")]
        public int RPH { get; set; }
    }
}
