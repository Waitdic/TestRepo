namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomStayCandidate
    {
        public RoomStayCandidate() { }
        [XmlArray(ElementName = "GuestCounts")]
        [XmlArrayItem(ElementName = "GuestCount")]
        public List<GuestCount> GuestCounts { get; set; } = new List<GuestCount>();
    }
}
