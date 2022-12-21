namespace iVectorOne.Suppliers.PremierInn.Models.Search
{
    using Common;
    using Soap;

    public class AvailabilityUpdateResponse : SoapContent
    {
        public SearchUpdateResponseParameters Parameters { get; set; } = new();
    }

    public class SearchUpdateResponseParameters : Parameters
    {
        public Session Session { get; set; } = new();

        public ErrorCode ErrorCode { get; set; } = new();
    }
}
