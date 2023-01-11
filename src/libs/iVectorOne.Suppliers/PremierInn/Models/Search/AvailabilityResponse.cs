namespace iVectorOne.Suppliers.PremierInn.Models.Search
{
    using System;
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class AvailabilityResponse : SoapContent
    {
        public SearchResponseParameters Parameters { get; set; } = new();
    }

    public class SearchResponseParameters : Parameters
    {
        public Session Session { get; set; } = new();

        [XmlElement("HotelDetails")]
        public HotelDetails[] HotelDetails { get; set; } = Array.Empty<HotelDetails>();
    }

    public class HotelDetails
    {
        public ErrorCode? ErrorCode { get; set; }

        [XmlAttribute]
        public string HotelCode { get; set; } = string.Empty;

        [XmlElement("RatePlan")]
        public RatePlan[] RatePlan { get; set; } = Array.Empty<RatePlan>();
    }
}
