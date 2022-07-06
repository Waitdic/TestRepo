using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    public class RoomRq
    {
        [XmlElement("RoomRate")]
        public RoomRate RoomRate { get; set; } = new();

        [XmlArray("Guests")]
        [XmlArrayItem("Guest")]
        public List<Guest> Guests { get; set; } = new();
        public bool ShouldSerializeGuests() => Guests?.Any() ?? false;
    }
}
