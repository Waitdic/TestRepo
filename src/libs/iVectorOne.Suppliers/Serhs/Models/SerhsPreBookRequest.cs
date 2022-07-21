namespace ThirdParty.CSSuppliers.Serhs.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("request")]
    public class SerhsPreBookRequest : BaseRequest
    {
        public SerhsPreBookRequest() { }

        public SerhsPreBookRequest(string version, string clientCode, string password, string branch, string tradingGroup,
            string languageCode) : base(version, clientCode, password, branch, tradingGroup, languageCode) { }

        [XmlAttribute("type")]
        public override string Type { get; set; } = "ACCOMMODATION_BOOKING";

        [XmlElement("accommodation")]
        public AccommodationRequest Accommodation { get; set; } = new();

        [XmlElement("period")]
        public Period Period { get; set; } = new();

        [XmlArray("rooms")]
        [XmlArrayItem("room")]
        public List<Room> Rooms { get; set; } = new();
    }
}
