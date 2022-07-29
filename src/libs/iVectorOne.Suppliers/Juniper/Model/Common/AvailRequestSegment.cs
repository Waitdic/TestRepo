namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class AvailRequestSegment
    {
        [XmlElement("StayDateRange")]
        public StayDateRange StayDateRange { get; set; } = new();

        [XmlArray("RoomStayCandidates")]
        [XmlArrayItem("RoomStayCandidate")]
        public List<RoomStayCandidate> RoomStayCandidates { get; set; } = new();

        [XmlArray("HotelSearchCriteria")]
        [XmlArrayItem("Criterion")]
        public List<Criterion> HotelSearchCriteria { get; set; } = new();
    }
}