namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class HotelReservation
    {
        public HotelReservation() { }

        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public List<RoomStay> RoomStays { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new();

        [XmlArray("ResGuests")]
        [XmlArrayItem("ResGuest")]
        public List<ResGuest> ResGuests { get; set; } = new();

        [XmlElement("ResGlobalInfo")]
        public ResGlobalInfo ResGlobalInfo { get; set; } = new();
    }
}
