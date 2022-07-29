using System.Xml.Serialization;

namespace iVectorOne.Suppliers.OceanBeds.Models.Common
{
    public class Filters
    {
        [XmlElement("isGamesRoom")]
        public string IsGamesRoom { get; set; } = string.Empty;

        [XmlElement("isSpaPool")]
        public string IsSpaPool { get; set; } = string.Empty;

        [XmlElement("isPrivatePool")]
        public string IsPrivatePool { get; set; } = string.Empty;

        [XmlElement("isOnGolfCourse")]
        public string IsOnGolfCourse { get; set; } = string.Empty;

        [XmlElement("isGatedCommunity")]
        public string IsGatedCommunity { get; set; } = string.Empty;

        [XmlElement("isPoolSouthFacing")]
        public string IsPoolSouthFacing { get; set; } = string.Empty;

        [XmlElement("isInternetAccess")]
        public string IsInternetAccess { get; set; } = string.Empty;

        [XmlElement("isConservationView")]
        public string IsConservationView { get; set; } = string.Empty;

        [XmlElement("isTheatre")]
        public string IsTheatre { get; set; } = string.Empty;

        public int AirportId { get; set; }
    }
}
