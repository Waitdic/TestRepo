namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    public class HotelBookingDetailRequest : SoapContent
    {
        public int BookingId { get; set; }

        public string ConfirmationNo { get; set; } = string.Empty;
    }
}
