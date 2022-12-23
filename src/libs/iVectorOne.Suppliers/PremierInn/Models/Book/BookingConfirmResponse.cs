namespace iVectorOne.Suppliers.PremierInn.Models.Book
{
    using Soap;
    using Common;

    public class BookingConfirmResponse : SoapContent
    {
        public BookResponseParameters Parameters { get; set; } = new();
    }

    public class BookResponseParameters : Parameters
    {
        public Session Session { get; set; } = new();

        public string ConfirmationNumber { get; set; } = string.Empty;

        public ErrorCode ErrorCode { get; set; } = new();
    }
}
