namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomType
    {
        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        public RoomDescription RoomDescription { get; set; } = new();

        public RoomTypeDescription RoomTypeDescription { get; set; } = new();

        [XmlArray("PropertyRoomBookings")]
        [XmlArrayItem("PropertyRoomBooking")]
        public PropertyRoomBooking[] PropertyRoomBookings { get; set; } = Array.Empty<PropertyRoomBooking>();
    }
}
