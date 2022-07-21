namespace iVectorOne.CSSuppliers.FastPayHotels.Models
{
    public class FastPayHotelsCancelRequest
    {
        public string messageID { get; set; } = string.Empty;
        public string bookingCode { get; set; } = string.Empty;
        public string customerCode { get; set; } = string.Empty;
    }
}