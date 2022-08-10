namespace iVectorOne.Suppliers.GoGlobal.Models
{
    public class BookingSearchRs : Main
    {
        public string BookingStatus { get; set; } = string.Empty;
        public string GoBookingCode { get; set; } = string.Empty;
        public string GoReference { get; set; } = string.Empty;
        public string CancellationDeadline { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public string ArrivalDate { get; set; } = string.Empty;
        public string TotalPrice { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }
}
