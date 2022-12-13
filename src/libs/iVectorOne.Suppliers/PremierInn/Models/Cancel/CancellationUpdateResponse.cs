namespace iVectorOne.Suppliers.PremierInn.Models.Cancel
{
    using iVectorOne.Suppliers.PremierInn.Models.Common;

    public class CancellationUpdateResponse
    {
        public CancelUpdateResponseParameters Parameters { get; set; } = new();
    }

    public class CancelUpdateResponseParameters : Parameters
    {
        public Session Session { get; set; } = new();

        public string CancellationNumber { get; set; } = string.Empty;

        public ErrorCode ErrorCode { get; set; } = new();
    }
}
