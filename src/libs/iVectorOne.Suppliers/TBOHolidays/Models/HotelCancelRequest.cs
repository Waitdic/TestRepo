namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using Common;

    public class HotelCancelRequest : SoapContent
    {
        public int BookingId { get; set; }

        public RequestType RequestType { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
}
