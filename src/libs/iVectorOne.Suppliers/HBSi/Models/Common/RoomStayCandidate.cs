namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomStayCandidate
    {
        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlAttribute("Quantity")]
        public int Quantity { get; set; }

        [XmlAttribute("RPH")]
        public int RPH { get; set; }

        [XmlAttribute("RatePlanCandidateRPH")]
        public int RatePlanCandidateRPH { get; set; }

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public List<GuestCount> GuestCounts { get; set; } = new List<GuestCount>();
    }
}
