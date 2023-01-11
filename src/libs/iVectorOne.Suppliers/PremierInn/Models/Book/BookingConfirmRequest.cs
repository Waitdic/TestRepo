namespace iVectorOne.Suppliers.PremierInn.Models.Book
{
    using Common;
    using iVectorOne.Suppliers.PremierInn.Models.Soap;

    public class BookingConfirmRequest : SoapContent
    {
        public Login Login { get; set; } = new();

        public BookRequestParameters Parameters { get; set; } = new();
    }

    public class BookRequestParameters : Parameters
    {
        public Session Session { get; set; } = new();

        public PaymentCard PaymentCard { get; set; } = new();

        public Rooms Rooms { get; set; } = new();

        public Address Address { get; set; } = new();

        public string BookingCompanyName { get; set; } = string.Empty;

        public BookerDetails BookerDetails { get; set; } = new();

        public string BookingType { get; set; } = string.Empty;

        public string ArrivalTime { get; set; } = string.Empty;

        public SpecialRequirements SpecialRequirements { get; set; } = new();
    }

    public class SpecialRequirements
    {
        public string Text { get; set; } = string.Empty;
    }
}
