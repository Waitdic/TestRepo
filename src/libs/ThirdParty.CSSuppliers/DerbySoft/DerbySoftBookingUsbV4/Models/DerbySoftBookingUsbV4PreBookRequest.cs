namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models
{
    using System.Collections.Generic;
    using ThirdParty.CSSuppliers.DerbySoft.Models;

    public class DerbySoftBookingUsbV4PreBookRequest
    {
        public Header header { get; set; }
        public ReservationIds reservationIds { get; set; }
        public string hotelId { get; set; }
        public StayRange stayRange { get; set; }
        public ContactPerson contactPerson { get; set; }
        public RoomCriteria roomCriteria { get; set; }
        public Total total { get; set; }
        public List<Guest> guests { get; set; }
        public List<RoomRate> roomRates { get; set; }
        
        public class ReservationIds
        {
            public string distributorResId { get; set; }
        }

        public class ContactPerson
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public int age { get; set; }
            public string type { get; set; }
            public string index { get; set; }
        }

        public class Total
        {
            public decimal amountAfterTax { get; set; }
            public decimal amountBeforeTax { get; set; }
        }

        public class Guest
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string type { get; set; }
            public string index { get; set; }
        }
    }
}