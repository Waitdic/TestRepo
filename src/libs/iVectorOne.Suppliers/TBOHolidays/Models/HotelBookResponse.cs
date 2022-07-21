namespace iVectorOne.CSSuppliers.TBOHolidays.Models
{
    using Common;

    public class HotelBookResponse : SoapContent
    {
        public BookingStatus BookingStatus { get; set; }

        public Status Status { get; set; } = new();

        public int BookingId { get; set; }

        public string ConfirmationNo { get; set; } = string.Empty;

        public int TripId { get; set; }
    }
}
