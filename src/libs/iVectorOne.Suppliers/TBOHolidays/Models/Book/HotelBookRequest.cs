namespace iVectorOne.Suppliers.TBOHolidays.Models.Book
{
    using System.Collections.Generic;
    using Common;

    public class HotelBookRequest
    {
        public string BookingCode { get; set; } = string.Empty;

        public List<CustomerDetail> CustomerDetails { get; set; } = new();

        public string ClientReferenceId { get; set; } = string.Empty;

        public string BookingReferenceId { get; set; } = string.Empty;

        public decimal TotalFare { get; set; }

        public string EmailId { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string BookingType { get; set; } = string.Empty;

        public string PaymentMode { get; set; } = string.Empty;

        public PaymentInfo PaymentInfo { get; set; } = new();
    }
}
