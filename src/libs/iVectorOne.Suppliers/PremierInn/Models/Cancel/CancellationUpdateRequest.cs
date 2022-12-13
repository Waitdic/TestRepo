namespace iVectorOne.Suppliers.PremierInn.Models.Cancel
{
    using iVectorOne.Suppliers.PremierInn.Models.Common;

    public class CancellationUpdateRequest
    {
        public Login Login { get; set; } = new();

        public CancelUpdateRequestParameters Parameters { get; set; } = new();
    }

    public class CancelUpdateRequestParameters : Parameters
    {
        public Session Session { get; set; } = new();

        public string ConfirmationNumber { get; set; } = string.Empty;

        public BookerDetails BookerDetails { get; set; } = new();
    }
}
