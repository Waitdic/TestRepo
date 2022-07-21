namespace ThirdParty.CSSuppliers.BedsWithEase.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class HotelAvailabilityRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;

        [XmlArray("Operators")]
        [XmlArrayItem("OperatorCode")]
        public string[] Operators { get; set; } = Array.Empty<string>();

        public StayDateRange StayDateRange { get; set; } = new();

        public RoomStayCandidate RoomStayCandidate { get; set; } = new();

        public HotelSearchCriterion HotelSearchCriterion { get; set; } = new();
    }
}
