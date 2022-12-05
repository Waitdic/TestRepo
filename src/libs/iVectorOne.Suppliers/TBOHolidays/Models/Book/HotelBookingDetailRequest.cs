namespace iVectorOne.Suppliers.TBOHolidays.Models.Book
{
    public class HotelBookingDetailRequest
    {
        public string ConfirmationNumber { get; set; } = string.Empty;

        public string BookingReferenceId { get; set; } = string.Empty;

        public string PaymentMode { get; set; } = string.Empty;
    }
}
