namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class BookingDeleteResponse : SoapContent
    {
        public CancelResponse Response { get; set; } = new();
    }
}
