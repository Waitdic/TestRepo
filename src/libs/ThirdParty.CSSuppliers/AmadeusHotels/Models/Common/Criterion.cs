namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Criterion
    {
        [XmlAttribute("ExactMatch")]
        public bool ExactMatch { get; set; }

        public HotelRef HotelRef { get; set; } = new();

        public StayDateRange StayDateRange { get; set; } = new();

        public RatePlanCandidates? RatePlanCandidates { get; set; }
        public bool ShouldSerializeRatePlanCandidates() => RatePlanCandidates != null;

        public RoomStayCandidates RoomStayCandidates { get; set; } = new();

        public CodeRef? CodeRef { get; set; }
        public bool ShouldSerializeCodeRef() => CodeRef != null;
    }
}
