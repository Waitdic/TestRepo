namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RoomStayCandidate
    {
        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;
        public bool ShouldSerializeRoomTypeCode() => !string.IsNullOrEmpty(RoomTypeCode);

        [XmlAttribute("RoomID")]
        public int RoomID { get; set; }
        public bool ShouldSerializeRoomID() => RoomID != 0;

        [XmlAttribute("Quantity")]
        public int Quantity { get; set; }

        [XmlAttribute("BookingCode")]
        public string BookingCode { get; set; } = string.Empty;
        public bool ShouldSerializeBookingCode() => BookingCode != string.Empty;

        public GuestCounts GuestCounts { get; set; } = new();
    }
}
