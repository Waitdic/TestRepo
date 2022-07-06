namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Room
    {
        [XmlAttribute("RPH")]
        public string RPH { get; set; } = string.Empty;

        public RoomType RoomType { get; set; } = new();

        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public List<RoomRate> RoomRates { get; set; } = new();

        [XmlArray("Guests")]
        [XmlArrayItem("Guest")]
        public List<Guest> Guests { get; set; } = new();
    }
}
