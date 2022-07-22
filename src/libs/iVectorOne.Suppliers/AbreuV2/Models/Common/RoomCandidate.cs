namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomCandidate
    {
        [XmlAttribute("RPH")]
        public string RPH { get; set; } = string.Empty;
        [XmlArray("Guests")]
        [XmlArrayItem("Guest")]
        public List<Guest> Guests { get; set; } = new();
    }
}
