namespace iVectorOne.Suppliers.PremierInn.Models.Cancel
{
    using iVectorOne.Suppliers.PremierInn.Models.Common;
    using iVectorOne.Suppliers.PremierInn.Models.Soap;

    public class CancellationUpdateResponse : SoapContent
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
