namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BookingHotel
    {
        [XmlElement("stayPeriod")]
        public StayPeriod StayPeriod { get; set; } = new();

        [XmlArray("rooms")]
        [XmlArrayItem("room")]
        public Room[] Rooms { get; set; } = Array.Empty<Room>();
    }
}
