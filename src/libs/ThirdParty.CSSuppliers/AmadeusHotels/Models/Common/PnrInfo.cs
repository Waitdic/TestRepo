﻿namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class PnrInfo
    {
        [XmlElement("reservationControlInfoPNR")]
        public ReservationControlInfoPNR ReservationControlInfoPNR { get; set; } = new();
    }
}