namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomStayCandidate
    {
        public RoomStayCandidate() { }

        [XmlAttribute("Quantity")]
        public int Quantity { get; set; }

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public List<GuestCount> GuestCounts { get; set; } = new();
    }
}
