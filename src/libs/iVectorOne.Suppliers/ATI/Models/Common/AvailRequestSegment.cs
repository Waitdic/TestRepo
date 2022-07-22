namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class AvailRequestSegment
    {
        public StayDateRange StayDateRange { get; set; } = new();

        [XmlArray("RoomStayCandidates")]
        [XmlArrayItem("RoomStayCandidate")]
        public RoomStayCandidate[] RoomStayCandidates { get; set; } = Array.Empty<RoomStayCandidate>();

        public HotelSearchCriteria HotelSearchCriteria { get; set; } = new();
    }
}
