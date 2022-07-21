namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RoomList
    {
        [XmlElement("markerRoomstayQuery")]
        public MarkerRoomstayQuery MarkerRoomstayQuery { get; set; } = new();

        [XmlElement("roomRateDetails")]
        public RoomRateDetails RoomRateDetails { get; set; } = new();

        [XmlElement("guaranteeOrDeposit")]
        public GuaranteeOrDeposit GuaranteeOrDeposit { get; set; } = new();

        [XmlElement("guestList")]
        public GuestList GuestList { get; set; } = new();
    }
}
