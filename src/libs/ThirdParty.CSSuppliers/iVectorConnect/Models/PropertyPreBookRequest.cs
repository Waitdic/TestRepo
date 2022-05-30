﻿namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PropertyPreBookRequest")]
    public class PropertyPreBookRequest
    {
        public LoginDetails? LoginDetails { get; set; }

        public string BookingToken { get; set; } = string.Empty;

        public string ArrivalDate { get; set; } = string.Empty;

        public int Duration { get; set; }

        [XmlArray("RoomBookings")]
        [XmlArrayItem("RoomBooking")]
        public RoomBooking[] RoomBookings { get; set; } = Array.Empty<RoomBooking>();
    }
}