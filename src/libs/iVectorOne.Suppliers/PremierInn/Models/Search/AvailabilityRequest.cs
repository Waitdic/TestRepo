namespace iVectorOne.Suppliers.PremierInn.Models.Search
{
    using System.Xml.Serialization;
    using Common;

    public class AvailabilityRequest
    {
        public Login Login { get; set; } = new();

        public SearchRequestParameters Parameters { get; set; } = new();
    }

    public class SearchRequestParameters : Parameters
    {
        public string HotelCode { get; set; } = string.Empty;

        public StayDateRange StayDateRange { get; set; } = new();

        [XmlElement("Rooms")]
        public Rooms Rooms { get; set; } = new();
    }
}
