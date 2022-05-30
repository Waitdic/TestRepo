﻿namespace ThirdPartyInterfaces.DerbySoft.DerbySoftBookingUsbV4.ThirdParty
{
    public class DerbySoftBookingUsbV4BookResponse
    {
        public ReservationIds reservationIds { get; set; }
        public class ReservationIds
        {
            public string distributorResId { get; set; }
            public string derbyResId { get; set; }
            public string supplierResId { get; set; }
        }

    }
}
