﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class ReservationControlInfoPNR
    {
        [XmlElement("reservation")]
        public Reservation Reservation { get; set; } = new();
    }
}
