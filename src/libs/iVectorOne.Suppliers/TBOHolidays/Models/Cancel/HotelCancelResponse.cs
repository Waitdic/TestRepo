namespace iVectorOne.Suppliers.TBOHolidays.Models.Cancel
{
    using Common;

    public class HotelCancelResponse
    {
        public Status Status { get; set; } = new();

        public string ConfirmationNumber { get; set; } = string.Empty;
    }
}
