namespace iVectorOne.Models.Property.Booking
{
    using System;
    using System.Collections.Generic;
    using iVectorOne.Models.Logging;

    public class Booking
    {
        public int BookingID { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string SupplierBookingReference { get; set; } = string.Empty;
        public int AccountID { get; set; }
        public int SupplierID { get; set; }
        public int PropertyID { get; set; }
        public BookingStatus Status { get; set; }
        public string LeadGuestName { get; set; } = string.Empty;
        public DateTime BookingDateTime { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Duration { get; set; }
        public decimal TotalPrice { get; set; }
        public int ISOCurrencyID { get; set; }
        public decimal EstimatedGBPPrice { get; set; }

        public List<APILog> APILogs { get; set; } = new();
        public List<SupplierAPILog> SupplierAPILogs { get; set; } = new();
    }
}