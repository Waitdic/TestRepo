﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class BookingInfo
    {
        [XmlElement("reservation")]
        public Reservation Reservation { get; set; } = new();
    }
}