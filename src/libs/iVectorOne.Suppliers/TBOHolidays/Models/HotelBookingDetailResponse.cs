namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using Common;

    public class HotelBookingDetailResponse : SoapContent
    {
        public Status Status { get; set; } = new();

        public BookingDetail BookingDetail { get; set; } = new();
    }
}
