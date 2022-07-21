namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelReservation
    {
        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public RoomStay[] RoomStays { get; set; } = Array.Empty<RoomStay>();

        [XmlArray("ResGuests")]
        [XmlArrayItem("ResGuest")]
        public ResGuest[] ResGuests { get; set; } = Array.Empty<ResGuest>();

        public UniqueId UniqueID { get; set; } = new();

        [XmlAttribute("ResStatus")]
        public string ResStatus { get; set; } = string.Empty;
    }
}
