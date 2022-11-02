namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class AvailRequestSegment
    {
        [XmlAttribute()]
        public string AvailReqType { get; set; } = null!;

        public StayDateRange StayDateRange { get; set; } = new();

        [XmlArray("RoomStayCandidates")]
        [XmlArrayItem("RoomStayCandidate")]
        public RoomStayCandidate[] RoomStayCandidates { get; set; } = Array.Empty<RoomStayCandidate>();

        public HotelSearchCriteria HotelSearchCriteria { get; set; } = new();
    }
}
