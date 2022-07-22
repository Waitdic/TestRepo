namespace iVectorOne.CSSuppliers.FastPayHotels.Models
{
    public class FastPayHotelsBookResponse
    {
        public string messageID { get; set; } = string.Empty;
        public Result result { get; set; } = new Result();
        
        public class Result
        {
            public bool success { get; set; }
            public BookingInfo bookingInfo { get; set; } = new BookingInfo();
        }

        public class BookingInfo
        {
            public string bookingCode { get; set; } = string.Empty;
        }
    }
}