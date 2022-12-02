namespace iVectorOne.Suppliers.TBOHolidays.Models.Cancel
{
    using Common;

    public class HotelCancelRequest
    {
        public int BookingId { get; set; }

        public RequestType RequestType { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
}
