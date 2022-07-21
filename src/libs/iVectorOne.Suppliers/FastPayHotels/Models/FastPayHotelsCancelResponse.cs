namespace iVectorOne.CSSuppliers.FastPayHotels.Models
{
    public class FastPayHotelsCancelResponse
    {
        public string messageID { get; set; } = string.Empty;
        public bool success { get; set; } 
        public string message { get; set; } = string.Empty;
    }
}