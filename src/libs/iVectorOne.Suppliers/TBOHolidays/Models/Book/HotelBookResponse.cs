namespace iVectorOne.Suppliers.TBOHolidays.Models.Book
{
    using Common;

    public class HotelBookResponse
    {
        public Status Status { get; set; } = new();

        public string ClientReferenceId { get; set; }

        public string ConfirmationNumber { get; set; } = string.Empty;
    }
}
