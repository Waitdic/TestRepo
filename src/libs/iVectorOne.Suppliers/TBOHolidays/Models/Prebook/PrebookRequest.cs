namespace iVectorOne.Suppliers.TBOHolidays.Models.Prebook
{
    using Common;

    public class PrebookRequest
    {
        public string BookingCode { get; set; } = string.Empty;

        public string PaymentMode { get; set; }
    }
}
