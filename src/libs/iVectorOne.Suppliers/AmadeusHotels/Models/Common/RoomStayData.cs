namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RoomStayData
    {
        [XmlElement("markerRoomStayData")]
        public MarkerRoomStayData MarkerRoomStayData { get; set; } = new();

        [XmlElement("globalBookingInfo")]
        public GlobalBookingInfo GlobalBookingInfo { get; set; } = new();

        [XmlElement("roomList")]
        public RoomList RoomList { get; set; } = new();

        [XmlElement("pnrInfo")]
        public PnrInfo PnrInfo { get; set; } = new();
    }
}
