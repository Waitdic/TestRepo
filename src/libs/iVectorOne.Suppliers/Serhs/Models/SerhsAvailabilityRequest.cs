namespace iVectorOne.Suppliers.Serhs.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("request")]
    public class SerhsAvailabilityRequest : BaseRequest
    {
        public SerhsAvailabilityRequest() { }

        public SerhsAvailabilityRequest(string version, string clientCode, string password, string branch, string tradingGroup,
            string languageCode) : base(version, clientCode, password, branch, tradingGroup, languageCode) { }

        [XmlAttribute("type")]
        public override string Type { get; set; } = "ACCOMMODATIONS_AVAIL";

        [XmlArray("searchCriteria")]
        [XmlArrayItem("criterion")]
        public List<Criterion> SearchCriteria { get; set; } = new();

        [XmlElement("period")]
        public Period Period { get; set; } = new();

        [XmlArray("rooms")]
        [XmlArrayItem("room")]
        public List<Room> Rooms { get; set; } = new();
    }
}
