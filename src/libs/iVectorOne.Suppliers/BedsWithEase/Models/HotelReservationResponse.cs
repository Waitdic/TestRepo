﻿namespace iVectorOne.Suppliers.BedsWithEase.Models
{
    using Common;

    public class HotelReservationResponse : SoapContent
    {
        public RsReservation RsReservation { get; set; } = new();
    }
}
