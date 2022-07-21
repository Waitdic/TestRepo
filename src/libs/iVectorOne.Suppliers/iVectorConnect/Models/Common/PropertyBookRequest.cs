namespace iVectorOne.Suppliers.iVectorConnect.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class PropertyBookRequest
    {
        public string BookingToken { get; set; } = string.Empty;

        public string ArrivalDate { get; set; } = string.Empty;

        public int Duration { get; set; }

        public decimal ExpectedTotal { get; set; }

        public string Request { get; set; } = string.Empty;

        [XmlArray("RoomBookings")]
        [XmlArrayItem("RoomBooking")]
        public RoomBooking[] RoomBookings { get; set; } = Array.Empty<RoomBooking>();
    }
}
