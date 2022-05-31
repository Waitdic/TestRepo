namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class HotelReservation
    {
        public HotelReservation() { }

        [XmlElement("UniqueID")]
        public UniqueId UniqueID { get; set; } = new();

        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public List<RoomStay> RoomStays { get; set; } = new();

        [XmlArray("ResGuests")]
        [XmlArrayItem("ResGuest")]
        public List<ResGuest> ResGuests { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public HotelReservationExtension HotelReservationExtension { get; set; } = new();
    }
}