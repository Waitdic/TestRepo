namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomStayCandidates
    {
        [XmlElement("RoomStayCandidate")]
        public RoomStayCandidate[] RoomStayCandidate { get; set; } = Array.Empty<RoomStayCandidate>();
    }
}
