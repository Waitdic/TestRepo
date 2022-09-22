namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class HotelReservation
    {
        [XmlAttribute("RoomStayReservation")]
        public string RoomStayReservation { get; set; } = string.Empty;

        [XmlAttribute("CreatorID")]
        public string CreatorID { get; set; } = string.Empty;

        [XmlAttribute("CreateDateTime")]
        public string CreateDateTime { get; set; } = string.Empty;

        [XmlAttribute("ResStatus")]
        public string ResStatus { get; set; } = string.Empty;
        public bool ShouldSerializeResStatus() => !string.IsNullOrEmpty(ResStatus);

        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();

        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public List<RoomStay> RoomStays { get; set; } = new();

        [XmlArray("ResGuests")]
        [XmlArrayItem("ResGuest")]
        public List<ResGuest> ResGuests { get; set; } = new();

        [XmlElement("ResGlobalInfo")]
        public ResGlobalInfo ResGlobalInfo { get; set; } = new();
    }
}
