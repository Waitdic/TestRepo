﻿namespace iVectorOne.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models
{
    using iVectorOne.CSSuppliers.DerbySoft.Models;

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