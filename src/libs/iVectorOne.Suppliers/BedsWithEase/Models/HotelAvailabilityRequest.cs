namespace iVectorOne.Suppliers.BedsWithEase.Models
{
    using Common;

    public class HotelAvailabilityRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;

        public StayDateRange StayDateRange { get; set; } = new();

        public RoomStayCandidate RoomStayCandidate { get; set; } = new();

        public HotelSearchCriterion HotelSearchCriterion { get; set; } = new();
    }
}
