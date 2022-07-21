namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class AvailRequestSegment
    {
        public AvailRequestSegment() { }
        [XmlElement(ElementName = "StayDateRange")]
        public StayDateRange StayDateRange { get; set; } = new StayDateRange();
        [XmlArray(ElementName = "RoomStayCandidates")]
        [XmlArrayItem(ElementName = "RoomStayCandidate")]
        public List<RoomStayCandidate> RoomStayCandidates { get; set; } = new List<RoomStayCandidate>();
        [XmlArray(ElementName = "HotelSearchCriteria")]
        [XmlArrayItem(ElementName = "Criterion")]
        public List<Criterion> HotelSearchCriteria { get; set; } = new List<Criterion>();
        [XmlElement(ElementName = "TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new TpaExtensions();
    }
}
