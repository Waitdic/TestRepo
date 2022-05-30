namespace ThirdPartyInterfaces.DerbySoft.DerbySoftBookingUsbV4.ThirdParty
{
    using DerbySoft.ThirdParty;

    public class DerbySoftBookingUsbV4CancelRequest
    {
        public Header header { get; set; }
        public ReservationIds reservationIds { get; set; }

        public class ReservationIds
        {
            public string distributorResId { get; set; }
            public string derbyResId { get; set; }
            public string supplierResId { get; set; }
        }

    }
}
