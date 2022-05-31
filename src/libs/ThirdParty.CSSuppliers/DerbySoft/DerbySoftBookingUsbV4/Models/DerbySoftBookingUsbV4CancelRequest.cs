namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models
{
    using ThirdParty.CSSuppliers.DerbySoft.Models;

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