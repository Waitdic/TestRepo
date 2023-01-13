namespace iVectorOne.Suppliers.TBOHolidays.Models.Book
{
    using Common;

    public class HotelBookingDetailResponse
    {
        public Status Status { get; set; } = new();

        public BookingDetail BookingDetail { get; set; } = new();
    }
}
