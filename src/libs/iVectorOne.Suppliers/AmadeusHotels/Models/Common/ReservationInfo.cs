﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class ReservationInfo
    {
        [XmlElement("reservation")]
        public Reservation Reservation { get; set; } = new();
    }
}
