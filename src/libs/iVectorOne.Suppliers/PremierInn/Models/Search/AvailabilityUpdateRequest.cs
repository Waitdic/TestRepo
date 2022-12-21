namespace iVectorOne.Suppliers.PremierInn.Models.Search
{
    using System.Xml.Serialization;
    using Common;

    public class AvailabilityUpdateRequest
    {
        public Login Login { get; set; } = new();

        public SearchUpdateRequestParameters Parameters { get; set; } = new();
    }

    public class SearchUpdateRequestParameters : Parameters
    {
        public Session Session { get; set; } = new();

        public string HotelCode { get; set; } = string.Empty;

        public StayDateRange StayDateRange { get; set; } = new();

        public string CellCode { get; set; } = string.Empty;

        public RatePlan RatePlan { get; set; } = new();

        [XmlElement("Rooms")]
        public Rooms Rooms { get; set; } = new();
    } 
}
