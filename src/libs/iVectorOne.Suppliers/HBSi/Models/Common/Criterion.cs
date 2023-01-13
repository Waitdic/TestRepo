namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Criterion
    {
        [XmlElement("StayDateRange")]
        public StayDateRange StayDateRange { get; set; } = new();

        [XmlArray("RatePlanCandidates")]
        [XmlArrayItem("RatePlanCandidate")]
        public List<RatePlanCandidate> RatePlanCandidates { get; set; } = new();

        [XmlArray("RoomStayCandidates")]
        [XmlArrayItem("RoomStayCandidate")]
        public List<RoomStayCandidate> RoomStayCandidates { get; set; } = new List<RoomStayCandidate>();
    }
}
