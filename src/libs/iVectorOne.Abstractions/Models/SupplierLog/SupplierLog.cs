namespace iVectorOne.Models.SupplierLog
{
    using Intuitive.Helpers.Net;

    public class SupplierLog
    {
        public string Title { get; set; } = string.Empty;
        public int BookingID { get; set; }
        public Request Request { get; set; } = null!;
    }
}