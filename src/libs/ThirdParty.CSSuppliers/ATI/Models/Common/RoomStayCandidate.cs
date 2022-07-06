namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomStayCandidate
    {
        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public GuestCount[] GuestCounts { get; set; } = Array.Empty<GuestCount>();
    }
}
